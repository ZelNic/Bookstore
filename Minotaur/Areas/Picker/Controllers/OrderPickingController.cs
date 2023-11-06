using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using Newtonsoft.Json;

namespace Minotaur.Areas.Picker.Controllers
{
    [Area("Picker"), Authorize(Roles = Roles.Role_Order_Picker)]
    public class OrderPickingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        // TODO: протестить

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

            string[] requiredOrderTypes = new string[] { StatusByOrder.Approved, StatusByOrder.InProcess, StatusByOrder.BuyerAgreesNeedSend };

            try
            {
                var assemblyOrders = (await _unitOfWork.Orders.GetAllAsync(s => requiredOrderTypes.Contains(s.OrderStatus)))
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
                              });

                return Json(new { data = assemblyOrders });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> TakeOrderOnAssebly(string orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));
                order.OrderStatus = StatusByOrder.InProcess;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

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
                order.OrderStatus = StatusByOrder.Approved;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;
                order.RefundAmount = order.PurchaseAmount;

                Notification notificationForAdminForRefund = new()
                {
                    OrderId = order.OrderId,
                    RecipientId = Guid.Parse("604c075d-c691-49d6-9d6f-877cfa866e59"),
                    SenderId = picker.WorkerId,
                    SendingTime = MoscowTime.GetTime(),
                    TypeNotification = NotificationSD.Refund,
                    Text = $"Необходимо осуществить возврат средств в сумме {order.AssemblyResponsibleWorkerId} за заказ под номером: {order.OrderId}."
                };
                await _unitOfWork.Notifications.AddAsync(notificationForAdminForRefund);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> PreorderCheck(string orderId, string? missingProduct = null)
        {
            try
            {
                Order? order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                Worker? picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));
                Office? stock = await _unitOfWork.Offices.GetAsync(o => o.Id == picker.OfficeId);

                if (missingProduct == "Отсутствуют")
                {
                    await SendCompletedOrder(order, picker, stock);
                }
                else if (order.MissingItems == missingProduct)
                {
                    await SendIncompleteOrder(order, picker, missingProduct, stock);
                }
                else
                {
                    await NotifyBuyerIncompleteOrder(order, picker, missingProduct);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<IActionResult> SendCompletedOrder(Order order, Worker picker, Office stock)
        {
            try
            {
                order.OrderStatus = StatusByOrder.Shipped;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;
                order.ShippedProducts = order.OrderedProducts;
                order.MissingItems = "Отсутствуют";

                string currentPointOrder = $"{stock.Name}, {stock.City}";
                List<string> pointList = new List<string>();
                pointList.Add(currentPointOrder);

                OrderMovementHistory orderMovementHistory = new()
                {
                    CurrentPosition = currentPointOrder,
                    HistoryOfСonversion = JsonConvert.SerializeObject(pointList),
                    OrderId = order.OrderId,
                    DispatchTime = MoscowTime.GetTime(),
                };

                Notification notification = new()
                {
                    OrderId = order.OrderId,
                    RecipientId = order.UserId,
                    SenderId = picker.WorkerId,
                    SendingTime = MoscowTime.GetTime(),
                    TypeNotification = StatusByOrder.Shipped,
                    Text = "Ваша заказ полностью собран и отправлен"
                };

                if (order.IsCourierDelivery == false)
                {
                    var workerPickUpPoin = await _unitOfWork.Workers.GetAllAsync(w => w.OfficeId == order.OrderPickupPointId);

                    Notification notificationForWorkerPickUpPoint = new()
                    {
                        OrderId = order.OrderId,
                        RecipientId = workerPickUpPoin.FirstOrDefault().UserId,
                        SenderId = picker.WorkerId,
                        SendingTime = MoscowTime.GetTime(),
                        TypeNotification = NotificationSD.SimpleNotification,
                        Text = $"Скоро в пункт будет доставлен заказ {order.OrderId}."
                    };
                    await _unitOfWork.Notifications.AddAsync(notificationForWorkerPickUpPoint);
                }

                await _unitOfWork.OrderMovementHistory.AddAsync(orderMovementHistory);
                await _unitOfWork.Notifications.AddAsync(notification);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<IActionResult> SendIncompleteOrder(Order order, Worker picker, string missingProduct, Office stock)
        {
            try
            {
                OrderProductData[]? misProd = JsonConvert.DeserializeObject<OrderProductData[]>(missingProduct);
                OrderProductData[]? orderedProducts = JsonConvert.DeserializeObject<OrderProductData[]>(order.OrderedProducts);
                List<OrderProductData>? shippedProducts = new();

                for (int i = 0; i < orderedProducts.Length; i++)
                {
                    bool isNeedAdd = true;
                    for (int j = 0; j < misProd.Length; j++)
                    {
                        if (misProd[j].Id == orderedProducts[i].Id && misProd[j].Count != orderedProducts[i].Count)
                        {
                            int countSend = orderedProducts[i].Count - misProd[j].Count;
                            if (countSend > 0)
                            {
                                shippedProducts.Add(new OrderProductData
                                {
                                    Id = orderedProducts[i].Id,
                                    Count = misProd[j].Count,
                                    Price = misProd[j].Price,
                                    ProductName = misProd[j].ProductName,
                                });
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

                order.OrderStatus = StatusByOrder.Shipped;
                order.AssemblyResponsibleWorkerId = picker.WorkerId;

                int orderedAmount = orderedProducts.Sum(p => p.Price * p.Count);
                int sheppedAmount = shippedProducts.Sum(p => p.Price * p.Count);

                order.RefundAmount = orderedAmount - sheppedAmount;

                Notification notificationForAdminForRefund = new()
                {
                    OrderId = order.OrderId,
                    RecipientId = Guid.Parse("604c075d-c691-49d6-9d6f-877cfa866e59"),
                    SenderId = picker.WorkerId,
                    SendingTime = MoscowTime.GetTime(),
                    TypeNotification = NotificationSD.Refund,
                    Text = $"Необходимо осуществить возврат средств в сумме {order.RefundAmount} за заказ под номером: {order.OrderId}."
                };

                Notification notification = new()
                {
                    OrderId = order.OrderId,
                    RecipientId = order.UserId,
                    SenderId = picker.WorkerId,
                    SendingTime = MoscowTime.GetTime(),
                    TypeNotification = StatusByOrder.Shipped,
                    Text = $"Ваша заказ полностью собран и отправлен"
                };

                string currentPointOrder = $"{stock.Name}, {stock.City}";
                List<string> pointList = new List<string>();
                pointList.Add(currentPointOrder);

                OrderMovementHistory orderMovementHistory = new()
                {
                    CurrentPosition = currentPointOrder,
                    HistoryOfСonversion = JsonConvert.SerializeObject(pointList),
                    OrderId = order.OrderId,
                    DispatchTime = MoscowTime.GetTime(),
                };

                await _unitOfWork.OrderMovementHistory.AddAsync(orderMovementHistory);

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.Notifications.AddAsync(notificationForAdminForRefund);
                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveAsync();
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<IActionResult> NotifyBuyerIncompleteOrder(Order order, Worker picker, string missingProduct)
        {
            try
            {
                OrderProductData[]? misProd = JsonConvert.DeserializeObject<OrderProductData[]>(missingProduct);

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

                order.OrderStatus = StatusByOrder.AwaitingConfirmationForIncompleteOrder;
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
                await _unitOfWork.Notifications.AddAsync(notification);
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
