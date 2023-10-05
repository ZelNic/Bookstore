using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class NotificationCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;
        public NotificationCustomerController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            IEnumerable<Notification> notifications = await _db.Notifications.Where(u => u.RecipientId == user.Id).Where(s => s.IsHidden == false).ToListAsync();

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
