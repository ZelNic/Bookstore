using Bookstore.DataAccess;
using Bookstore.Models.Models;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class NotificationCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public NotificationCustomerController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        public IActionResult Delete(int notificationId)
        {
            Notification notification = _db.Notifications.Find(notificationId);
            if (notification != null)
            {
                _db.Notifications.Remove(notification);
                _db.SaveChanges();
            }               

            return RedirectToAction("Index");
        }
    }
}
