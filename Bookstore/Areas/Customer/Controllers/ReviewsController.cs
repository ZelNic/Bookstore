using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Areas.Customer
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ReviewsController(ApplicationDbContext db)
        {
            _db = db;            
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
