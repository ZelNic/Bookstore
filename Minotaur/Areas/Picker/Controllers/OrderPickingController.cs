using DocumentFormat.OpenXml.Spreadsheet;
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


        public async Task<IActionResult> GetAssemblyOrders()
        {
            var worker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserAsync(User).Result.Id));

            var assemblyOrders = _unitOfWork.Orders.GetAllAsync().Result
                                .Where(s => s.OrderStatus == StatusByOrder.StatusApproved_1)
                                .Select(s => new
                                {
                                    s.OrderId,
                                    s.UserId,
                                    Products = JsonConvert.DeserializeObject<List<OrderProductData>>(s.ProductData)
                                        .Join(_unitOfWork.Products.GetAllAsync().Result, o => o.Id, p => p.ProductId, (o, p) => new
                                        {
                                            p.ProductId,
                                            p.Name,
                                            p.Author,
                                            o.Count,
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


        public async Task<IActionResult> SendCollectedOrder(string orderId, string? missingProduct = null)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetAsync(i => i.OrderId == Guid.Parse(orderId));
                var picker = await _unitOfWork.Workers.GetAsync(w => w.UserId == Guid.Parse(_userManager.GetUserId(User)));

                if (missingProduct == null)
                {
                    order.OrderStatus = StatusByOrder.StatusShipped_3;
                    order.AssemblyResponsibleWorkerId = picker.WorkerId;
                }
                else
                {
                    var misProd = JsonConvert.DeserializeObject<DataMissingProductByPicker[]>(missingProduct);

                    string nameProducts = "";
                    for (int i = 0; i < misProd.Length; i++)
                    {
                        if (i + 1 < misProd.Length)
                        {
                            nameProducts += misProd[i].ProductName + ", ";
                        }
                        else
                        {
                            nameProducts += misProd[i].ProductName + ".";
                        }
                    }

                    order.OrderStatus = StatusByOrder.AwaitingConfirmationForIncompleteOrder_7;
                    Notification notification = new()
                    {
                        OrderId = Guid.Parse(orderId),
                        RecipientId = order.UserId,
                        SenderId = picker.WorkerId,
                        SendingTime = MoscowTime.GetTime(),
                        TypeNotification = NotificationSD.IncompleteOrderType,
                        Text = $"Здравствуйте, к сожалению, на складе закончились следующие товары, которые Вы заказывал: {nameProducts}." +
                        $" Согласны Вы получить не полный заказ? Будет осуществлен возврат средств за отсутствующие товары."
                    };
                    _unitOfWork.Notifications.AddAsync(notification);
                }

                _unitOfWork.Orders.Update(order);

                _unitOfWork.SaveAsync();
                return Ok();
            }
            catch
            {
                return BadRequest("Произошла ошибка");
            }
        }

    }
}
