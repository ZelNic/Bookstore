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
        public IActionResult AddBasket(int productId, bool isWishList = false)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                var basket = _db.ShoppingBasket.Where(u => u.UserId == _user.UserId)
                    .Where(u => u.ProductId == productId)
                    .FirstOrDefault();

                if (basket != null)
                {
                    var count = basket.CountProduct;
                    count++;
                    basket.CountProduct = count;
                    _db.ShoppingBasket.Update(basket);
                }

                if (basket == null)
                {
                    ShoppingBasket newProductInShopBasket = new()
                    {
                        ProductId = productId,
                        UserId = _user.UserId,
                        CountProduct = 1
                    };

                    _db.ShoppingBasket.Add(newProductInShopBasket);
                }


                _db.SaveChanges();

                if (isWishList == true)
                {
                    return RedirectToAction("Index", "WishList", new { area = "Customer" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "Customer" });
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveFromBasket(int productId, bool delete = false, bool minus = false, bool plus = false)
        {
            ShoppingBasket productInBasket = _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).Where(u => u.ProductId == productId).FirstOrDefault();
            int countProductInBasket = productInBasket.CountProduct;

            if (delete == true || countProductInBasket == 1)
            {
                _db.ShoppingBasket.Remove(productInBasket);

                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (minus == true)
            {
                countProductInBasket--;
            }
            else if (plus == true)
            {
                countProductInBasket++;
            }

            productInBasket.CountProduct = countProductInBasket;
            _db.ShoppingBasket.Update(productInBasket);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
