using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using System.Diagnostics;
using Telegram.Bot.Types;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<MinotaurUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<MinotaurUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetProductsData(int numberPage = 1)
        {
            int countProductOnPage = 6;

            var productsAll = await _unitOfWork.Products.GetAllAsync();
            int countRecords = productsAll.Count();

            int totalPages = (int)Math.Ceiling((float)countRecords / countProductOnPage);

            var productData = productsAll.Skip((numberPage - 1) * countProductOnPage).Take(countProductOnPage)
                .Join(_unitOfWork.Categories.GetAll(), p => p.Category, c => c.Id, (p, c) => new
                {
                    p.ProductId,
                    p.ImageURL,
                    p.Name,
                    p.Author,
                    p.Price,
                    Category = c.Name,
                    isInWishList = false,
                    isInShoppingBasket = false
                }).ToList();

            var user = await GetDataByUser();

            if (user != null)
            {
                var productInWL = await GetDataByWishlistUser(user.Id);
                var productInSB = await GetDataByShoppingBasketUser(user.Id);

                productData = productData.Select(product => new
                {
                    product.ProductId,
                    product.ImageURL,
                    product.Name,
                    product.Author,
                    product.Price,
                    product.Category,
                    isInWishList = productInWL.Contains(product.ProductId),
                    isInShoppingBasket = productInSB.Contains(product.ProductId),
                }).ToList();
            }

            return Json(new { data = productData, totalPages });
        }

        private async Task<List<int>> GetDataByWishlistUser(string userId)
        {
            List<int> productInWL = new();

            var wishList = await _unitOfWork.WishLists.GetAsync(w => w.UserId == userId);

            if (wishList != null)
            {
                productInWL = wishList.ProductId.Split('|').Select(int.Parse).ToList();
            }

            return productInWL;
        }

        private async Task<List<int>> GetDataByShoppingBasketUser(string userId)
        {
            List<int> productInSB = new();
            var shoppingBasket = await _unitOfWork.ShoppingBaskets.GetAllAsync(w => w.UserId == Guid.Parse(userId));
            var activeSB = shoppingBasket.Where(s => s.IsPurchased == false).FirstOrDefault();
            if (activeSB != null)
            {
                productInSB = ShoppingBasketController.ParseProductData(activeSB.ProductIdAndCount).Keys.ToList();
            }

            return productInSB;
        }

        private async Task<MinotaurUser> GetDataByUser()
        {
            MinotaurUser? user = await _userManager.GetUserAsync(User);
            return user;
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _unitOfWork.Products.GetAsync(p => p.ProductId == productId);

            var user = await GetDataByUser();
            var productInWL = await GetDataByWishlistUser(user.Id);
            var productInSB = await GetDataByShoppingBasketUser(user.Id);

            if (product != null)
            {
                return View(product);
            }
            return BadRequest("Страница продукта не найдена");
        }


        public async Task<IActionResult> Search(string? searchString)
        {
            if (searchString == null)
            {
                return RedirectToAction("Index");
            }
            List<Product> products = _unitOfWork.Products.GetAllAsync(product => product.Name.Contains(searchString.ToLower())).Result.ToList();

            return View(products);
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
};