using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class WishListController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly User _user;

        public WishListController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
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

        [HttpPost]
        public async Task<IActionResult> AddWishList(int productId)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                var wishList = await _db.WishLists.Where(u => u.UserId == _user.UserId)
                    .Where(u => u.ProductId == productId)
                    .FirstOrDefaultAsync();

                if (wishList != null)
                {
                    _db.WishLists.Update(wishList);
                }
                else
                {
                    WishList newProductInWishList = new()
                    {
                        ProductId = productId,
                        UserId = _user.UserId,
                    };

                    await _db.WishLists.AddAsync(newProductInWishList);
                }

                await _db.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWL(int productId)
        {
            var productOnRemove = await _db.WishLists.Where(u => u.UserId == _user.UserId).Where(u=>u.ProductId == productId).FirstOrDefaultAsync();
            _db.WishLists.Remove(productOnRemove);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "WishList", new { area = "Customer" });
        }
    }
}
