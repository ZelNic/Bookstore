using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            return View();
        }

        public async Task<IActionResult> GetWishList()
        {
            if(_user == null)
            {
                return Json(new { data = "Log In" });
            }
            

            WishList? wishList = await _db.WishLists.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

            if(wishList == null)
            {
                return Json(new { data = "WishList not found." });
            }

            List<int>? listId = wishList.ProductId.Split('|').Select(int.Parse).ToList();

            var wishListJson = await _db.Books
                .Where(u => listId.Contains(u.BookId))
                .Join(_db.Categories, b => b.Category, c => c.Id, (b, c) => new
                {
                    productId = b.BookId,
                    nameProduct = b.Title,
                    category = c.Name,
                    image = b.ImageURL,
                    author = b.Author,
                    price = b.Price
                })
                .ToListAsync();


            return Json(new { data = wishListJson });
        }




        [HttpPost]
        public async Task<IActionResult> AddWishList(int productId)
        {
            if (_user == null)
            {
                return BadRequest(new { error = "Пользователь не вошел в систему." });
            }


            var wishList = await _db.WishLists.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

            if (wishList != null)
            {
                string[] productIds = wishList.ProductId.Split('|');

                foreach (string id in productIds)
                {
                    if (id == productId.ToString())
                        return Ok();
                }

                wishList.ProductId += "|" + productId.ToString();

                _db.WishLists.Update(wishList);
            }
            else
            {
                WishList newProductInWishList = new()
                {
                    ProductId = productId.ToString(),
                    UserId = _user.UserId,
                };

                await _db.WishLists.AddAsync(newProductInWishList);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishList(int productId)
        {
            WishList? wishList = _db.WishLists.Where(i => i.UserId == _user.UserId).FirstOrDefault();
            if (wishList == null)
                return NotFound();

            List<string> listId = wishList.ProductId.Split('|').ToList();
            listId.Remove(productId.ToString());
            wishList.ProductId = string.Join("|", listId);

            if (string.IsNullOrWhiteSpace(wishList.ProductId))
            {
                _db.WishLists.Remove(wishList);
            }
            else
            {
                _db.WishLists.Update(wishList);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
