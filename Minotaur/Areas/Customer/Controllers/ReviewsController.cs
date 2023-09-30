using Minotaur.DataAccess;
using Minotaur.Models;
using Microsoft.AspNetCore.Mvc;

namespace Minotaur.Areas.Customer
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
