using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Stockkeeper
{
    [Area("Stockkeeper")]
    public class StockController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _storekeeper;
        public StockController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Employees.Where(u => u.UserId == userId) != null))
                {
                    _storekeeper = _db.User.Find(userId);
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            StockVM stockVM = new()
            {
                Stock = await _db.Stocks.Where(u => u.ResponsiblePerson == _storekeeper.UserId).ToListAsync(),
                Books = await _db.Books.ToListAsync()
            };
            return View(stockVM);
        }

        [HttpPost]
        public async Task<List<Book>> GetProductAsync(string? nameProduct = null, int? productId = null)
        {
            List<Book> books = new();
            if (nameProduct != null)
            {
                books = await _db.Books.Where(book => book.Title.Contains(nameProduct.ToLower())).ToListAsync();
            }
            else if (productId != null)
            {
                books = await _db.Books.Where(u => u.BookId == productId).ToListAsync();
            }

            return books;
        }
    }
}
