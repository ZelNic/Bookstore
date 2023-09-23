﻿using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[CustomAuthorization(ApplicationDbContext, IHttpContextAccessor)]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly bool _isAdmin;

        public BookController(ApplicationDbContext db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                CustomAuthorizationAttribute checkUserIsAdmin = new();
                _isAdmin = checkUserIsAdmin.CheckUserIsAdmin(_db, _contextAccessor);
            }
        }

        public async Task<IActionResult> Index()
        {
            if (_isAdmin == false)
                return NotFound("Отказано в доступе");

            if (_contextAccessor.HttpContext.Session.GetInt32("Username") != null)
            {
                int? userId = _contextAccessor.HttpContext.Session.GetInt32("Username");
                if ((userId != null) && (_db.Employees.Find(userId) != null))
                {
                    List<Product> booksList = await _db.Products.ToListAsync();
                    List<Category> categoriesList = await _db.Categories.ToListAsync();

                    ProductVM bookVM = new()
                    {
                        ProductsList = booksList,
                        CategoriesList = categoriesList
                    };

                    return View(bookVM);
                }
                else
                {
                    return NotFound("Отказано в доступе.");
                }
            }
            else
            {
                return NotFound("Отказано в доступе.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? bookId)
        {
            var book = new Product();

            if (bookId != null)
            {
                book = await _db.Products.FindAsync(bookId);
            }

            ProductVM bookVM = new()
            {
                Product = book,
                CategoriesList = _db.Categories.ToList()
            };

            return View(bookVM);
        }



        [HttpPost]
        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPost(ProductVM productVM)
        {
            if (productVM.Product.ProductId == 0)
            {
                _db.Products.Add(productVM.Product);
            }
            else
            {
                var oldVersionBook = await _db.Products.FindAsync(productVM.Product.ProductId);

                if (oldVersionBook != null)
                {
                    oldVersionBook.Title = productVM.Product.Title;
                    oldVersionBook.Author = productVM.Product.Author;
                    oldVersionBook.ISBN = productVM.Product.ISBN;
                    oldVersionBook.Description = productVM.Product.Description;
                    oldVersionBook.Price = productVM.Product.Price;
                    oldVersionBook.Category = productVM.Product.Category;
                    oldVersionBook.ImageURL = productVM.Product.ImageURL;
                    _db.Products.Update(oldVersionBook);
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Book");
        }



        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                var bookOnDelete = await _db.Products.FindAsync(id);
                return View(bookOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            var productOnDelete = await _db.Products.FindAsync(product.ProductId);
            if (productOnDelete != null)
            {
                _db.Products.Remove(productOnDelete);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Book");
            }
            else
            {
                return NotFound();
            }
        }
    }
}

