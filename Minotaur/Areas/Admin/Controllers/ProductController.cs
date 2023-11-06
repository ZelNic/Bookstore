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

        //TODO: переписать старницу с продуктами для админа, добавить поиск и частичную загрузку страниц
        public async Task<IActionResult> Index()
        {
            ProductVM productVM = new()
            {
                ProductsList = (await _unitOfWork.Products.GetAllAsync()).ToList(),
                CategoriesList = (await _unitOfWork.Categories.GetAllAsync()).ToList(),
            };

            return View(productVM);
        }


        [HttpGet]
        public async Task<IActionResult> DetailsProduct(int? productId)
        {
            ProductVM bookVM = new()
            {
                Product = await _unitOfWork.Products.GetAsync(u => u.ProductId == productId),
                CategoriesList = await _unitOfWork.Categories.GetAllAsync()
            };

            return View(bookVM);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateOrAdd(string dataProduct)
        {
            Product? product = JsonConvert.DeserializeObject<Product>(dataProduct);
            if (product == null) { return BadRequest("Ошибка в отправленных данных"); }

            if (product.ProductId == 0)
            {
                await _unitOfWork.Products.AddAsync(product);
            }
            else
            {
                var oldVersionBook = await _unitOfWork.Products.GetAsync(p => p.ProductId == product.ProductId);

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

            await _unitOfWork.SaveAsync();
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int productId)
        {
            var productOnDelete = await _unitOfWork.Products.GetAsync(p => p.ProductId == productId);
            if (productOnDelete != null)
            {
                _unitOfWork.Products.Remove(productOnDelete);
                await _unitOfWork.SaveAsync();
                return Ok();
            }

            return BadRequest("Товар не найден");
        }
    }
}

