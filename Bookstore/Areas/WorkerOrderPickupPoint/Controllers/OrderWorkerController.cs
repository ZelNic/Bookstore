using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Models.ViewModel;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint")]
    public class OrderWorkerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public OrderWorkerController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.User.Find(userId) != null))
                {
                    _user = _db.User.Find(userId);
                }
            }
        }

        public IActionResult Order(string operation)
        {
            if (operation == null)
            {
                return NotFound("Такой операции нет.");
            }

            switch (operation)
            {
                case SD.ViewOrderDetails:
                    ViewBag.operation = SD.ViewOrderDetails;
                    break;
                case SD.AcceptOrder:
                    ViewBag.operation = SD.AcceptOrder;
                    break;
                case SD.IssueOrder:
                    ViewBag.operation = SD.IssueOrder;
                    break;
                default:
                    return NotFound(operation);
            }

            return View();
        }

        [HttpPost]
        public IActionResult Order(int searchOrderId, string operation)
        {
            OrderVM orderVM = new()
            {
                Order = _db.Order.Find(searchOrderId),
                OperationName = operation
            };

            if (orderVM.Order == null)
            {
                return NotFound(SD.NotFoundUser);
            }


            return View(orderVM);
        }

        //Пока не используется, надо сделать для приема товара смену кода
        public IActionResult Deliver(int orderId, int? statusCode)
        {
            Order? order = _db.Order.Find(orderId);
            if (order != null)
            {
                switch (statusCode)
                {
                    case 0:
                        order.OrderStatus = SD.StatusPending_0;
                        break;
                    case 1:
                        order.OrderStatus = SD.StatusApproved_1;
                        break;
                    case 2:
                        order.OrderStatus = SD.StatusInProcess_2;
                        break;
                    case 3:
                        order.OrderStatus = SD.StatusShipped_3;
                        break;
                    case 4:
                        order.OrderStatus = SD.StatusDelivered_4;
                        break;
                    case 5:
                        order.OrderStatus = SD.StatusCancelled_5;
                        break;
                    case 6:
                        order.OrderStatus = SD.StatusRefunded_6;
                        break;
                }
                _db.Order.Update(order);
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "Orders", new { area = "Customer" });
        }


        [HttpPost]
        public IActionResult IssuePackage(int orderId)
        {
            var order = _db.Order.Find(orderId);

            if (order == null)
            {
                return NotFound(SD.NotFoundOrder);
            }

            Random random = new Random();
            int confirmationСode = random.Next(100000, 999999);

            Notification notification = new()
            {
                OrderId = orderId,
                RecipientId = _user.UserId,
                SenderId = order.UserId,
                SendingTime = MoscowTime.GetTime(),
                Text = NotificationSD.IssueCode + ' ' + confirmationСode + ' ' + "Скажите его оператору для выдачи заказа."
            };

            order.ConfirmationСode = confirmationСode;
            
            _db.Order.Update(order);
            _db.Notifications.Add(notification);
            _db.SaveChanges();

            return View();
        }


    }
}
