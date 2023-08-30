using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Reflection.Metadata.BlobBuilder;

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
            Stock? stock = _db.Stocks.Where(u => u.ResponsiblePerson == _stockkeeper.UserId).FirstOrDefault();
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

        [HttpPost]
        public async Task<bool> AddProductInStock(int productId, int numberShelf, int productCount)
        {
            if (await _db.Books.FindAsync(productId) == null)
            {
                return false;
            }

            var stock = await _db.Stocks.Where(u => u.ResponsiblePerson == _stockkeeper.UserId).FirstOrDefaultAsync();

            if (stock != null)
            {
                if (stock.ProductId == productId && stock.ShelfNumber == numberShelf)
                {
                    stock.Count += productCount;
                    _db.Stocks.Update(stock);
                }
                else
                {
                    Stock receipt = new()
                    {
                        City = stock.City,
                        Street = stock.Street,
                        ResponsiblePerson = stock.ResponsiblePerson,
                        ProductId = productId,
                        ShelfNumber = numberShelf,
                        Count = productCount
                    };
                    await _db.AddAsync(receipt);
                }

                await _db.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }

        public async Task<IActionResult> ChangeShelfProduct(int id)
        {


            return Ok();
        }

        public async Task<IActionResult> GetStock()
        {
            var stock = await _db.Stocks.Where(u => u.ResponsiblePerson == _stockkeeper.UserId)
                .Join(_db.Books, s => s.ProductId, b => b.BookId, (s, b) => new
                {
                    productId = s.ProductId,
                    count = s.Count,
                    shelfNumber = s.ShelfNumber,
                    nameProduct = b.Title
                }).ToListAsync();

            return Json(new { data = stock });
        }
    }
}
