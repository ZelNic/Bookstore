using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            List<Category> categoryList = _unitOfWork.Categories.GetAll().ToList();
            return View(categoryList);
        }

        public async Task<IActionResult> BookCategory(int categoryId)
        {
            CategoryVM categoryVM = new()
            {
                BookList = _unitOfWork.Products.GetAll(u => u.Category == categoryId).ToList(),
                CategoryList = _unitOfWork.Categories.GetAll().ToList()
            };

            return View(categoryVM);
        }

        public async Task<IActionResult> Upsert(int? categoryId)
        {
            if (categoryId == 0 || categoryId == null)
            {
                Category category = new();

                return View(category);
            }

            var book = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryId);

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (category.Id == 0)
            {
                _unitOfWork.Categories.Update(category);
            }
            else
            {
                _unitOfWork.Categories.Update(category);
            }

            _unitOfWork.SaveAsync();
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? categoryId)
        {
            if (categoryId != null)
            {
                var categoryOnDelete = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryId);
                return View(categoryOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? categoryId)
        {
            var categoryOnDelete = await _unitOfWork.Categories.GetAsync(c => c.Id == categoryId);
            if (categoryOnDelete != null)
            {
                _unitOfWork.Categories.Remove(categoryOnDelete);
                _unitOfWork.SaveAsync();
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
