using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class ShoppingBasketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public ShoppingBasketController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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
            if (_user != null)
            {
                return View(_user);
            }
            return NotFound();
        }

        [HttpPost]
        public void AddBasket(int productId)
        {
            ShoppingBasket newProductInShopBasket = new()
            {
                BasketId = _user.UserId,
                ProductId = productId,
                UserId = _user.UserId,
                CountProduct = 1
            };
            _db.ShoppingBasket.Add(newProductInShopBasket);
            _db.SaveChanges();
        }
    }
}





//if (bookId == null)
//{
//    return View();
//}

//var book = _db.Books.FirstOrDefault(u => u.BookId == bookId);
//if (book == null)
//{
//    return NotFound();
//}
//return View(book);