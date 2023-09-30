using Minotaur.DataAccess;
using Minotaur.Models.Models;
using Minotaur.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Minotaur.Areas.Customer
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

            if (_contextAccessor.HttpContext.Session.GetInt32("UserId") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("UserId");
                if ((userId != null) && (_db.User.Find(userId) != null))
                {
                    _user = _db.User.Find(userId);
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Notification> notifications = await _db.Notifications.Where(u => u.RecipientId == _user.UserId).Where(s => s.IsHidden == false).ToListAsync();

            return View(notifications);
        }

        public async Task<IActionResult> Delete(int notificationId)
        {
            Notification? notification = await _db.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _db.Notifications.Remove(notification);
                await _db.SaveChangesAsync();
            }               

            return RedirectToAction("Index");
        }
    }
}
