using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Bookstore.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.WorkerOrderPickupPoint
{
    [Area("WorkerOrderPickupPoint")]
    public class NotificationWorkerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public NotificationWorkerController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        public IActionResult Index()
        {
            IEnumerable<Notification> notifications = _db.Notifications.Where(u => u.RecipientId == _user.UserId).Where(s => s.IsHidden == false);

            return View(notifications);
        }

        [HttpPost]
        public void Send(int notificationСode, int orderId)
        {
            string text = "";
            Order order = _db.Order.Find(orderId);
            switch (notificationСode)
            {
                case 0:
                    text = NotificationSD.TechnicalProblems_0;
                    break;
                case 1:
                    text = NotificationSD.PaymentNotPassed_1;
                    break;
                case 2:
                    text = NotificationSD.PaymentPassed_2;
                    break;
                case 3:
                    text = NotificationSD.OrderIsGoing_3;
                    break;
                case 4:
                    text = NotificationSD.OrderSent_4;
                    break;
                case 5:
                    text = NotificationSD.OrderArrived_5;
                    break;
                case 6:
                    text = NotificationSD.CourierDelivery_6;
                    break;
                case 7:
                    text = NotificationSD.OrderСancelled_7;
                    break;
                default:
                    NotFound("Неверная команда.");
                    break;
            }

            Notification notification = new()
            {
                SenderId = _user.UserId,
                RecipientId = order.UserId,
                OrderId = order.OrderId,
                Text = text,
                IsHidden = false,
                SendingTime = MoscowTime.GetTime(),
            };


            _db.Notifications.Add(notification);
            _db.SaveChanges();
        }
    }
}
