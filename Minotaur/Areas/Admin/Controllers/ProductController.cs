using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;
using System.Linq;

namespace Minotaur.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Roles.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> productsList =  _unitOfWork.Products.GetAll().ToList();
            List<Category> categoriesList =  _unitOfWork.Categories.GetAll().ToList();

            ProductVM productVM = new()
            {
                ProductsList = productsList,
                CategoriesList = categoriesList,
            };

            return View(productVM);
        }




        [HttpGet]
        public async Task<IActionResult> Upsert(int? productId)
        {
            var product = new Product();

            if (productId != null)
            {
                product = await _unitOfWork.Products.GetAsync(u => u.ProductId == productId);
            }

            ProductVM bookVM = new()
            {
                Product = product,
                CategoriesList = _unitOfWork.Categories.GetAll().ToList()   
            };

            return View(bookVM);
        }



        [HttpPost]
        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPost(ProductVM productVM)
        {
            if (productVM.Product.ProductId == 0)
            {
                _unitOfWork.Products.AddAsync(productVM.Product);
            }
            else
            {
                var oldVersionBook = await _unitOfWork.Products.GetAsync(p => p.ProductId == productVM.Product.ProductId);

                if (oldVersionBook != null)
                {
                    oldVersionBook.Name = productVM.Product.Name;
                    oldVersionBook.Author = productVM.Product.Author;
                    oldVersionBook.ISBN = productVM.Product.ISBN;
                    oldVersionBook.Description = productVM.Product.Description;
                    oldVersionBook.Price = productVM.Product.Price;
                    oldVersionBook.Category = productVM.Product.Category;
                    oldVersionBook.ImageURL = productVM.Product.ImageURL;
                    _unitOfWork.Products.Update(oldVersionBook);
                }
            }

            _unitOfWork.SaveAsync();
            return Ok();
        }



        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                var productOnDelete = await _unitOfWork.Products.GetAsync(p=>p.ProductId == id);
                return View(productOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            var productOnDelete = await _unitOfWork.Products.GetAsync(p=>p.ProductId == product.ProductId);
            if (productOnDelete != null)
            {
                _unitOfWork.Products.Remove(productOnDelete);
                _unitOfWork.SaveAsync();
                return RedirectToAction("Index", "Book");
            }
            else
            {
                return NotFound();
            }
        }
    }
}

