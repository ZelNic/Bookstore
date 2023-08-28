using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Stockkeeper
{
    [Area("Stockkeeper")]
    public class StockController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _stockkeeper;
        public StockController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Employees.Where(u => u.UserId == userId) != null))
                {
                    if (_db.User.Find(userId) != null)
                    {
                        _stockkeeper = _db.User.Find(userId);
                    }
                }
                else
                {
                    NotFound(SD.AccessDenied);
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            StockVM stockVM = new()
            {
                Stock = await _db.Stocks.Where(u => u.ResponsiblePerson == _stockkeeper.UserId).ToListAsync(),
                Books = await _db.Books.ToListAsync()
            };
            return View(stockVM);
        }

        [HttpPost]
        public async Task<bool> AddProductInStock(int productId, int numberShelf, int productCount = 1)
        {


            return true;
        }
    }
}
