using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public OrdersController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        public async Task<IActionResult> Index()
        {
            OrderVM orderVM = new()
            {
                Orders = await _db.Order.Where(u => u.UserId == _user.UserId).ToListAsync(),
            };
            return View(orderVM);
        }
    }
}
