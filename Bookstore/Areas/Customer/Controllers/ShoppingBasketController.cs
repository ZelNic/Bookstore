using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index()
        {
            if (_user != null)
            {
                return View(_user);
            }
            return NotFound();
        }

        //add in the basket or in the wish list
        [HttpPost]
        public async Task<IActionResult> AddBasket(int productId, bool isWishList = false)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                var basket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId)
                    .Where(u => u.ProductId == productId)
                    .FirstOrDefaultAsync();

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

                    await _db.ShoppingBasket.AddAsync(newProductInShopBasket);
                }

                await _db.SaveChangesAsync();

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
        public async Task<IActionResult> RemoveFromBasket(int productId)
        {
            ShoppingBasket productInBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).Where(u => u.ProductId == productId).FirstOrDefaultAsync();
            int countProductInBasket = productInBasket.CountProduct;

            _db.ShoppingBasket.Remove(productInBasket);

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeCountProduct(int productId, bool minus = false, bool plus = false)
        {
            ShoppingBasket productInBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).Where(u => u.ProductId == productId).FirstOrDefaultAsync();
            int countProductInBasket = productInBasket.CountProduct;

            if (countProductInBasket == 1 && minus == true)
            {
                _db.ShoppingBasket.Remove(productInBasket);
            }
            else
            {
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
            }     
            
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
