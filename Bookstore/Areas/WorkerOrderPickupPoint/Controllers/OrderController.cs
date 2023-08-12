using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public OrderController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        public IActionResult Order()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Order(int searchOrderId)
        {
            Order order = _db.Order.Find(searchOrderId);

            return View(order);
        }


        public IActionResult Deliver(int orderId, int? statusCode)
        {
            Order order = _db.Order.Find(orderId);
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
    }
}
