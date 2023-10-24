using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    [Authorize(Roles = Roles.Role_Customer)]
    public class ShoppingBasketController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public ShoppingBasketController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> GetShoppingBasket()
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = _unitOfWork.ShoppingBaskets.GetAllAsync(u => u.UserId == Guid.Parse(user.Id)).Result.Where(n => n.IsPurchased == false).FirstOrDefault();

            if (shoppingBasket == null)
            {
                return BadRequest("Пустая корзина");
            }

            Dictionary<int, int> productIdAndCount = ParseProductData(shoppingBasket.ProductIdAndCount);

            var sb = _unitOfWork.Products.GetAllAsync().Result.Where(u => productIdAndCount.Keys.Contains(u.ProductId))
           .Join(_unitOfWork.Categories.GetAllAsync().Result, b => b.Category, c => c.Id, (b, c) => new
           {
               image = b.ImageURL,
               nameProduct = b.Name,
               author = b.Author,
               category = c.Name,
               price = b.Price,
               productId = b.ProductId,
               count = productIdAndCount[b.ProductId],
               isSelect = false
           }).ToList();

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
        public async Task<IActionResult> AddToBasketProduct(int productId)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = _unitOfWork.ShoppingBaskets.GetAllAsync().Result.Where(u => u.UserId == Guid.Parse(user.Id)).Where(n => n.IsPurchased == false).FirstOrDefault();

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
                _unitOfWork.ShoppingBaskets.Update(shoppingBasket);
            }
            else
            {
                ShoppingBasket newShoppingBasket = new()
                {
                    UserId = Guid.Parse(user.Id),
                    ProductIdAndCount = productId.ToString() + ":1"
                };

                _unitOfWork.ShoppingBaskets.AddAsync(newShoppingBasket);
            }

            _unitOfWork.Save();

            return Ok();
        }



        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(string productsId)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = _unitOfWork.ShoppingBaskets.GetAll(u => u.UserId == Guid.Parse(user.Id)).Where(n => n.IsPurchased == false).FirstOrDefault();
            if (shoppingBasket == null) { return BadRequest("Пустая корзина"); }

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
                _unitOfWork.ShoppingBaskets.Remove(shoppingBasket);
                _unitOfWork.Save();
                return BadRequest("Пустая корзина");
            }
            else
            {
                shoppingBasket.ProductIdAndCount = SerializationProductData(productIdAndCount);
                _unitOfWork.Save();
                _unitOfWork.ShoppingBaskets.Update(shoppingBasket);
                return Ok();
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangeCountProduct(string productData)
        {
            MinotaurUser user = await _userManager.GetUserAsync(User);

            ShoppingBasket? shoppingBasket = _unitOfWork.ShoppingBaskets.GetAll(u => u.UserId == Guid.Parse(user.Id)).Where(n => n.IsPurchased == false).FirstOrDefault();

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
            _unitOfWork.ShoppingBaskets.Update(shoppingBasket);
            _unitOfWork.Save();

            return Ok();
        }
    }
}
