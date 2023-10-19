using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Models.Supporting_Models;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.Picker.Controllers
{
    [Area("Picker"), Authorize(Roles = Roles.Role_Order_Picker)]
    public class OrderPickingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public OrderPickingController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetOrders()
        {
            var worker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));

            string[] requiredOrderTypes = new string[] { StatusByOrder.StatusApproved_1,StatusByOrder.StatusInProcess_2,
                                                         StatusByOrder.BuyerAgreesNeedSend_8,StatusByOrder.StatusCancelled_5,
                                                         StatusByOrder.BuyerDontAgreesNeedRefunded_9, StatusByOrder.StatusCancelled_10 };

            try
            {
                var assemblyOrders = _unitOfWork.Orders.GetAll().Where(s => requiredOrderTypes.Contains(s.OrderStatus))
                              .Select(s => new
                              {
                                  s.OrderId,
                                  s.UserId,
                                  s.OrderStatus,
                                  s.MissingItems,
                                  Products = JsonConvert.DeserializeObject<List<OrderProductData>>(s.OrderedProducts)
                                      .Join(_unitOfWork.Products.GetAll(), o => o.Id, p => p.ProductId, (o, p) => new
                                      {
                                          p.ProductId,
                                          p.Name,
                                          p.Author,
                                          o.Count,
                                          o.Price,
                                          IsChecked = false,
                                      }),
                                  PurchaseDate = s.PurchaseDate.ToString("dd.MM.yyyy HH:mm"),
                                  AssemblyResponsibleWorkerId = worker.WorkerId,
                                  DataStock = _unitOfWork.StockMagazine.GetAllAsync(u => u.StockId == worker.OfficeId).Result
                                      .Where(record => record.Operation != OperationStock.ApplicationForPurchaseOfGoods)
                                      .Select(record => new
                                      {
                                          record.ProductId,
                                          record.Count,
                                          record.ShelfNumber
                                      }).ToList(),
                              }).ToList();

                return Json(new { data = assemblyOrders });
            }
            catch
            {
                return BadRequest("Заказов нет");
            }

        }

        public async Task<IActionResult> TakeOrderOnAssebly(string orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));
                order.OrderStatus = StatusByOrder.StatusInProcess_2;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;

                _unitOfWork.Orders.Update(order);
                _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        public async Task<IActionResult> CancelAsseblyOrder(string orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));
                order.OrderStatus = StatusByOrder.StatusApproved_1;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;

                _unitOfWork.Orders.Update(order);
                _unitOfWork.Save();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> SendCollectedOrder(string orderId, string? missingProduct = null)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));

                if (missingProduct == "Отсутствуют")
                {
                    order.OrderStatus = StatusByOrder.StatusShipped_3;
                    order.AssemblyResponsibleWorkerId = picker.WorkerId;
                    order.ShippedProducts = order.OrderedProducts;
                    order.MissingItems = missingProduct;

                    Notification notification = new()
                    {
                        OrderId = order.OrderId,
                        RecipientId = order.UserId,
                        SenderId = picker.WorkerId,
                        SendingTime = MoscowTime.GetTime(),
                        TypeNotification = StatusByOrder.StatusShipped_3,
                        Text = $"Ваша заказ полностью собран и отправлен"
                    };

                    _unitOfWork.Notifications.AddAsync(notification);
                }
                else if (order.MissingItems == missingProduct)
                {
                    order.OrderStatus = StatusByOrder.StatusShipped_3;
                    order.AssemblyResponsibleWorkerId = picker.WorkerId;
                    order.ShippedProducts = order.OrderedProducts;
                    order.MissingItems = missingProduct;

                    OrderProductData[]? misProd = JsonConvert.DeserializeObject<OrderProductData[]>(missingProduct);
                    OrderProductData[]? orderedProducts = JsonConvert.DeserializeObject<OrderProductData[]>(order.OrderedProducts);
                    List<OrderProductData>? shippedProducts = new();

                    for (int i = 0; i < orderedProducts.Length; i++)
                    {
                        bool isNeedAdd = true;
                        for (int j = 0; j < misProd.Length; j++)
                        {
                            if (misProd[j].Id == orderedProducts[i].Id)
                            {
                                int countSend = orderedProducts[i].Count - misProd[j].Count;
                                if (countSend > 0)
                                {
                                    orderedProducts[i].Count = countSend;
                                    shippedProducts.Add(orderedProducts[i]);
                                    isNeedAdd = false;
                                    break;
                                }
                                else
                                {
                                    isNeedAdd = false;
                                    break;
                                }
                            }
                        }
                        if (isNeedAdd == true)
                            shippedProducts.Add(orderedProducts[i]);
                    }

                    order.ShippedProducts = JsonConvert.SerializeObject(shippedProducts);

                    Notification notification = new()
                    {
                        OrderId = order.OrderId,
                        RecipientId = order.UserId,
                        SenderId = picker.WorkerId,
                        SendingTime = MoscowTime.GetTime(),
                        TypeNotification = StatusByOrder.StatusShipped_3,
                        Text = $"Ваша заказ полностью собран и отправлен"
                    };

                    _unitOfWork.Notifications.AddAsync(notification);
                }
                {
                    var misProd = JsonConvert.DeserializeObject<OrderProductData[]>(missingProduct);

                    string misProductNameAndCount = "";
                    for (int i = 0; i < misProd.Length; i++)
                    {
                        if (i + 1 < misProd.Length)
                        {
                            misProductNameAndCount += misProd[i].ProductName + ", " + misProd[i].Count + ", ";
                        }
                        else
                        {
                            misProductNameAndCount += misProd[i].ProductName + ", " + misProd[i].Count + ".";
                        }

                    }

                    order.OrderStatus = StatusByOrder.AwaitingConfirmationForIncompleteOrder_7;
                    order.MissingItems = missingProduct;

                    Notification notification = new()
                    {
                        OrderId = order.OrderId,
                        RecipientId = order.UserId,
                        SenderId = picker.WorkerId,
                        SendingTime = MoscowTime.GetTime(),
                        TypeNotification = NotificationSD.IncompleteOrderType,
                        Text = $"Здравствуйте, к сожалению, на складе закончились следующие товары, которые Вы заказывали: {misProductNameAndCount}" +
                        $" Будет осуществлен возврат средств за отсутствующие товары. Согласны Вы получить не полный заказ?"
                    };
                    _unitOfWork.Notifications.AddAsync(notification);
                }

                _unitOfWork.Orders.Update(order);

                _unitOfWork.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
