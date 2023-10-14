using Minotaur.DataAccess;
using Minotaur.Models;
using Minotaur.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace Minotaur.Areas.Customer
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<MinotaurUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<MinotaurUser> userManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ProductVM productVM = await GetProductsVM();
            return View(productVM);
        }

        public async Task<ProductVM> GetProductsVM()
        {
            List<Product>? productsList = await _db.Products.ToListAsync();
            List<Category>? categoriesList = await _db.Categories.ToListAsync();
            WishList? wishLists = null;
            ShoppingBasketClient? shoppingBasketClient = null;

            MinotaurUser? user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                wishLists = await _db.WishLists.Where(u => u.UserId == user.Id).FirstOrDefaultAsync();
                ShoppingBasket? sb = await _db.ShoppingBaskets.Where(u => u.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();
                if (sb != null)
                {
                    shoppingBasketClient = new()
                    {
                        Id = sb.UserId,
                        ProductIdAndCount = ShoppingBasketController.ParseProductData(sb.ProductIdAndCount)
                    };
                }
            }

            ProductVM bookVM = new()
            {
                User = user?.Id,
                ProductsList = productsList,
                CategoriesList = categoriesList,
                WishList = wishLists,
                ShoppingBasket = shoppingBasketClient
            };

            return bookVM;
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _db.Products.FindAsync(productId);

            return View(product);
        }


        public async Task<IActionResult> Search(string? searchString)
        {
            if (searchString == null)
            {
                return RedirectToAction("Index");
            }
            IEnumerable<Product> products = await _db.Products.Where(book => book.Name.Contains(searchString.ToLower())).ToListAsync();

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
}