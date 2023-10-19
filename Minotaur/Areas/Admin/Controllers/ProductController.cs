using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.SD;
using Newtonsoft.Json;

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
            List<Product> productsList = _unitOfWork.Products.GetAllAsync().Result.ToList();
            List<Category> categoriesList = _unitOfWork.Categories.GetAllAsync().Result.ToList();

            ProductVM productVM = new()
            {
                ProductsList = productsList,
                CategoriesList = categoriesList,
            };

            return View(productVM);
        }


        [HttpGet]
        public async Task<IActionResult> DetailsProduct(int? productId)
        {
            var product = new Product();

            if (productId != null)
            {
                product = await _unitOfWork.Products.GetAsync(u => u.ProductId == productId);
            }

            ProductVM bookVM = new()
            {
                Product = product,
                CategoriesList = _unitOfWork.Categories.GetAllAsync().Result.ToList()
            };           

            return View(bookVM);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateOrAdd(string dataProduct)
        {
            Product? product = JsonConvert.DeserializeObject<Product>(dataProduct);
            if(product == null) { return BadRequest("Ошибка в отправленных данных"); }

            if (product.ProductId == 0)
            {
                _unitOfWork.Products.AddAsync(product);
            }
            else
            {
                var oldVersionBook =  await _unitOfWork.Products.GetAsync(p => p.ProductId == product.ProductId);

                if (oldVersionBook != null)
                {
                    oldVersionBook.Name = product.Name;
                    oldVersionBook.Author = product.Author;
                    oldVersionBook.ISBN = product.ISBN;
                    oldVersionBook.Description = product.Description;
                    oldVersionBook.Price = product.Price;
                    oldVersionBook.Category = product.Category;
                    oldVersionBook.ImageURL = product.ImageURL;
                    oldVersionBook.EditorId = product.EditorId;
                    _unitOfWork.Products.Update(oldVersionBook);
                }
            }

            _unitOfWork.Save();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var productOnDelete =  await _unitOfWork.Products.GetAsync(p => p.ProductId == productId);
            if (productOnDelete != null)
            {
                _unitOfWork.Products.Remove(productOnDelete);
                _unitOfWork.Save();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

