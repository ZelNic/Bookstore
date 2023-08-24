using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bookstore.Areas.Customer
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, ApplicationDbContext db)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<Book> booksList = await _db.Books.ToListAsync();
            List<Category> categoriesList = await _db.Categories.ToListAsync();

            BookVM bookVM = new()
            {
                BooksList = booksList,
                CategoriesList = categoriesList
            };

            return View(bookVM);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _db.Books.FindAsync(productId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string? searchString)
        {            
            if(searchString == null)
            {
                return RedirectToAction("Index");
            }
            IEnumerable<Book> books = await _db.Books.Where(book => book.Title.Contains(searchString.ToLower())).ToListAsync();

            return View(books);
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