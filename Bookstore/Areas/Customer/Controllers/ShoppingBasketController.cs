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
        public IActionResult AddBasket(int productId)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                ShoppingBasket newProductInShopBasket = new()
                {
                    ProductId = productId,
                    UserId = _user.UserId,
                    CountProduct = 1
                };

                _db.ShoppingBasket.Add(newProductInShopBasket);
                _db.SaveChanges();


                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
        }

        [HttpPost]
        public IActionResult RemoveFromBasket(int productId)
        {
            var basketId = _db.ShoppingBasket.Where(u => u.UserId == _user.UserId);
            if (basketId != null)
            {
                var productOnRemove = _db.ShoppingBasket.First(p => p.ProductId == productId);
                _db.ShoppingBasket.Remove(productOnRemove);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");     
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