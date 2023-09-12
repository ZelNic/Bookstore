using Bookstore.DataAccess;
using Bookstore.Models;
using Bookstore.Models.Models;
using Bookstore.Models.SD;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;

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
            List<Book>? booksList = await _db.Books.ToListAsync();
            List<Category>? categoriesList = await _db.Categories.ToListAsync();
            WishList? wishLists = new();

            if (_user != null)
            {
                wishLists = await _db.WishLists.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            }

            BookVM bookVM = new()
            {
                BooksList = booksList,
                CategoriesList = categoriesList,
                WishList = wishLists,
                User = _user
            };

            return View(bookVM);
        }


        [HttpPost]
        public async Task<IActionResult> AddWishList(int productId)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
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
        public async Task<IActionResult> RemoveFromWL(int productId)
        {
            WishList? wishList = _db.WishLists.Where(i=>i.UserId == _user.UserId).FirstOrDefault();
            if (wishList == null)            
                return NotFound();

            string result = wishList.ProductId.Replace("|" + productId.ToString(), "");

             _db.WishLists.Update(wishList);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
