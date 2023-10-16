using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Minotaur.Models.SD;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingBasketController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public ShoppingBasketController(ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> GetShoppingBasket()
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();

            if (shoppingBasket == null)
            {
                return BadRequest("Пустая корзина");
            }

            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket.ProductIdAndCount);


            var sb = await _db.Products.Where(u => productIdAndCount.Keys.Contains(u.ProductId))
                .Join(_db.Categories, b => b.Category, c => c.Id, (b, c) => new
                {
                    image = b.ImageURL,
                    nameProduct = b.Name,
                    author = b.Author,
                    category = c.Name,
                    price = b.Price,
                    productId = b.ProductId,
                    count = productIdAndCount[b.ProductId],
                    isSelect = false
                }).ToListAsync();

            return Json(new { data = sb });
        }


        public static Dictionary<int, int> ParseProductData(string shoppingBasket)
        {
            List<string>? productData = new List<string> { };
            Dictionary<int, int>? productIdAndCount = null;

            if (shoppingBasket == null)
            {
                return productIdAndCount;
            }
            else
            {
                productIdAndCount = new Dictionary<int, int> { };
            }

            if (shoppingBasket.Contains('|') == true)
            {
                productData = shoppingBasket.Split('|').ToList();
            }
            else
            {
                productData.Add(shoppingBasket);
            }


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

        public static string SerializationProductData(Dictionary<int, int> productIdAndCount)
        {
            string result = string.Join("|", productIdAndCount.Select(data => $"{data.Key}:{data.Value}"));
            Console.WriteLine(result);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddBasket(int productId, bool isFromWishList = false)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();

            if (shoppingBasket != null)
            {
                Dictionary<int, int>? productIdAndCount = ParseProductData(shoppingBasket.ProductIdAndCount);

                if (productIdAndCount == null)
                {
                    return BadRequest("Корзина пуста");
                }

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
                _db.ShoppingBaskets.Update(shoppingBasket);
            }
            else
            {
                ShoppingBasket newShoppingBasket = new()
                {
                    UserId = Guid.Parse(user.Id),
                    ProductIdAndCount = productId.ToString() + ":1"
                };

                await _db.ShoppingBaskets.AddAsync(newShoppingBasket);
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

        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(string productsId)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();
            if (shoppingBasket == null) { return BadRequest(); }

            List<int> listProductsId = productsId.Split(',').Select(int.Parse).ToList();

            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket.ProductIdAndCount);

            foreach (int product in listProductsId)
            {
                if (productIdAndCount.ContainsKey(product))
                {
                    productIdAndCount.Remove(product);
                }
            }

            if (productIdAndCount.Count == 0)
            {
                _db.ShoppingBaskets.Remove(shoppingBasket);
                await _db.SaveChangesAsync();
                return BadRequest(new { error = "Пустая корзина" }); ;
            }
            else
            {
                shoppingBasket.ProductIdAndCount = SerializationProductData(productIdAndCount);
                await _db.SaveChangesAsync();
                _db.ShoppingBaskets.Update(shoppingBasket);
                return Ok();
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangeCountProduct(string productData)
        {           
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();

            if (shoppingBasket == null) { return BadRequest(); }

            Dictionary<int, int> oldProductIdAndCount = ParseProductData(shoppingBasket.ProductIdAndCount);

            Dictionary<int, int> newProductIdAndCount = ParseProductData(productData);

            foreach (var product in newProductIdAndCount)
            {
                if (oldProductIdAndCount.ContainsKey(product.Key))
                {
                    oldProductIdAndCount[product.Key] = newProductIdAndCount[product.Key];
                }
            }

            shoppingBasket.ProductIdAndCount = SerializationProductData(oldProductIdAndCount);
            _db.ShoppingBaskets.Update(shoppingBasket);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
