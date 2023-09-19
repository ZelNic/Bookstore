﻿using Bookstore.DataAccess;
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
            return View();
        }
        public async Task<IActionResult> GetShoppingBasket()
        {
            ShoppingBasket? shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

            if (shoppingBasket == null)
            {
                return BadRequest();
            }

            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket);


            var sb = await _db.Books.Where(u => productIdAndCount.Keys.Contains(u.BookId))
                .Join(_db.Categories, b => b.Category, c => c.Id, (b, c) => new
                {
                    image = b.ImageURL,
                    nameProduct = b.Title,
                    author = b.Author,
                    category = c.Name,
                    price = b.Price,
                    productId = b.BookId,
                    count = productIdAndCount[b.BookId],
                    isSelect = false
                }).ToListAsync();

            return Json(new { data = sb });
        }


        public static Dictionary<int, int> ParseProductData(ShoppingBasket shoppingBasket)
        {
            List<string> productData = new();

            if (shoppingBasket.ProductIdAndCount.Contains('|') == true)
            {
                productData = shoppingBasket.ProductIdAndCount.Split('|').ToList();
            }
            else
            {
                productData.Add(shoppingBasket.ProductIdAndCount);
            }

            Dictionary<int, int> productIdAndCount = new Dictionary<int, int>();

            foreach (string idAndCount in productData)
            {
                string[] parts = idAndCount.Split(':');
                if (parts.Length == 2)
                {
                    int key = Convert.ToInt32(parts[0]);
                    int value = Convert.ToInt32(parts[1]);
                    productIdAndCount.Add(key, value);
                }
            }

            return productIdAndCount;
        }

        public string SerializationProductData(Dictionary<int, int> productIdAndCount)
        {
            string result = string.Join("|", productIdAndCount.Select(kv => $"{kv.Key}:{kv.Value}"));
            Console.WriteLine(result);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddBasket(int productId, bool isFromWishList = false)
        {
            if (_user == null)
            {
                return RedirectToAction("LogIn", "User", new { area = "Identity" });
            }
            else
            {
                ShoppingBasket shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();

                if (shoppingBasket != null)
                {
                    Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket);

                    if (productIdAndCount.ContainsKey(productId))
                    {
                        int count;
                        productIdAndCount.TryGetValue(productId, out count);
                        productIdAndCount[productId] = count + 1;
                    }
                    else
                    {
                        productIdAndCount.Add(productId, 1);
                        shoppingBasket.ProductIdAndCount = SerializationProductData(productIdAndCount);
                    }
                    _db.ShoppingBasket.Update(shoppingBasket);
                }
                else
                {
                    ShoppingBasket newShoppingBasket = new()
                    {
                        UserId = _user.UserId,
                        ProductIdAndCount = productId.ToString() + ":1"
                    };

                    await _db.ShoppingBasket.AddAsync(newShoppingBasket);
                }

                await _db.SaveChangesAsync();

                if (isFromWishList == true)
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
            ShoppingBasket? shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return BadRequest(); }
            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket);

            if (productIdAndCount.ContainsKey(productId))
            {
                productIdAndCount.Remove(productId);
            }

            shoppingBasket.ProductIdAndCount = SerializationProductData(productIdAndCount);

            _db.ShoppingBasket.Update(shoppingBasket);
            await _db.SaveChangesAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> ChangeCountProduct(int productId, bool minus = false, bool plus = false)
        {
            ShoppingBasket shoppingBasket = await _db.ShoppingBasket.Where(u => u.UserId == _user.UserId).FirstOrDefaultAsync();
            
            if (shoppingBasket == null) { return BadRequest(); }
            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket);

            if (productIdAndCount.ContainsKey(productId))
            {
                if (productIdAndCount[productId] == 1 && minus == true)
                {
                    productIdAndCount.Remove(productId);
                }
                else
                {
                    if (minus == true)
                    {
                        productIdAndCount[productId]--;
                    }
                    else if (plus == true)
                    {
                        productIdAndCount[productId]++;
                    }

                    shoppingBasket.ProductIdAndCount = SerializationProductData(productIdAndCount); 

                    _db.ShoppingBasket.Update(shoppingBasket);
                }
            }                     

            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
