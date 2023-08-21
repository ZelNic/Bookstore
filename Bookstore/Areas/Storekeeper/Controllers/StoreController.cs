using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Storekeeper
{
    [Area("Storekeeper")]
    public class StoreController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _storekeeper;
        public StoreController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Employees.Find(userId) != null))
                {
                    _storekeeper = _db.User.Find(userId);
                }
            }
        }

        public IActionResult Index()
        {
            IQueryable stocks = _db.Stocks.Where(u => u.ResponsiblePerson == _storekeeper.UserId);
            return View(stocks);
        }
    }
}
