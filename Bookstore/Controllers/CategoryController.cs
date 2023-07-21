using Microsoft.AspNetCore.Mvc;
using Bookstore.Models;

namespace Bookstore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _db.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                Category newCategory = new Category();
                return View(newCategory);
            }
            else
            {
                var category = _db.Categories.FirstOrDefault(c => c.Id == id);
                if(category == null)
                {
                    return NotFound();
                }
                return View(category);
            }
        }
    }
}
