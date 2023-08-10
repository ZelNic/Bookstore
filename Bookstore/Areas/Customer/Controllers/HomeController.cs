using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            List<Book> booksList = _db.Books.ToList();
            List<Category> categoriesList = _db.Categories.ToList();

            BookVM bookVM = new()
            {
                BooksList = booksList,
                CategoriesList = categoriesList
            };

            return View(bookVM);
        }

        public IActionResult Details(int productId)
        {
            var product = _db.Books.Find(productId);

            return View(product);
        }

        [HttpPost]
        public IActionResult Search(string searchString)
        {            
            IEnumerable<Book> books = _db.Books.Where(book => book.Title.Contains(searchString.ToLower()));

            return View(books);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}