using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public WishlistController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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
        public IActionResult AddWishList(int productId)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                var wishList = _db.WishLists.Where(u => u.UserId == _user.UserId)
                    .Where(u => u.ProductId == productId)
                    .FirstOrDefault();

                if (wishList != null)
                {
                    var count = wishList.CountProduct;
                    _db.WishLists.Update(wishList);
                }
                else
                {
                    WishList newProductInWishList = new()
                    {
                        ProductId = productId,
                        UserId = _user.UserId,
                        CountProduct = 1
                    };

                    _db.WishLists.Add(newProductInWishList);
                }

                _db.SaveChanges();

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
        }

        [HttpPost]
        public IActionResult AddWishList(int productId)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                var wishList = _db.WishLists.Where(u => u.UserId == _user.UserId)
                    .Where(u => u.ProductId == productId)
                    .FirstOrDefault();

                if (wishList != null)
                {
                    var count = wishList.CountProduct;
                    _db.WishLists.Update(wishList);
                }
                else
                {
                    WishList newProductInWishList = new()
                    {
                        ProductId = productId,
                        UserId = _user.UserId,
                        CountProduct = 1
                    };

                    _db.WishLists.Add(newProductInWishList);
                }

                _db.SaveChanges();

                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
        }
    }
}
