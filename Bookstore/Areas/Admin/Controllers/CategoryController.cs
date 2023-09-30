using Minotaur.DataAccess;
using Minotaur.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categoryList = await _db.Categories.ToListAsync();
            return View(categoryList);
        }

        public async Task<IActionResult> BookCategory(int categoryId)
        {
            CategoryVM categoryVM = new()
            {
                BookList = await _db.Products.Where(u => u.Category == categoryId).ToListAsync(),
                CategoryList = await _db.Categories.ToListAsync()
            };

            return View(categoryVM);
        }

        public async Task<IActionResult> Upsert(int? categoryId)
        {
            if (categoryId == 0 || categoryId == null)
            {
                Category category = new ();

                return View(category);
            }

            var book = await _db.Categories.FindAsync(categoryId);

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (category.Id == 0)
            {
                _db.Add(category);
            }
            else
            {
                _db.Update(category);
            }

            _db.SaveChanges();
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? categoryId)
        {
            if (categoryId != null)
            {
                var categoryOnDelete = await _db.Categories.FindAsync(categoryId);
                return View(categoryOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? categoryId)
        {
            var categoryOnDelete = await _db.Categories.FindAsync(categoryId);
            if (categoryOnDelete != null)
            {
                _db.Categories.Remove(categoryOnDelete);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index","Category");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
