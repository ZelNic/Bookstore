using Bookstore.DataAccess;
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
                    List<Book> booksList = await _db.Books.ToListAsync();
                    List<Category> categoriesList = await _db.Categories.ToListAsync();

                    BookVM bookVM = new()
                    {
                        BooksList = booksList,
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
            var book = new Book();

            if (bookId != null)
            {
                book = await _db.Books.FindAsync(bookId);
            }

            BookVM bookVM = new()
            {
                Book = book,
                CategoriesList = _db.Categories.ToList()
            };

            return View(bookVM);
        }



        [HttpPost]
        [ActionName("Upsert")]
        public async Task<IActionResult> UpsertPost(BookVM bookVM)
        {
            if (bookVM.Book.BookId == 0)
            {
                _db.Books.Add(bookVM.Book);
            }
            else
            {
                var oldVersionBook = await _db.Books.FindAsync(bookVM.Book.BookId);

                if (oldVersionBook != null)
                {
                    oldVersionBook.Title = bookVM.Book.Title;
                    oldVersionBook.Author = bookVM.Book.Author;
                    oldVersionBook.ISBN = bookVM.Book.ISBN;
                    oldVersionBook.Description = bookVM.Book.Description;
                    oldVersionBook.Price = bookVM.Book.Price;
                    oldVersionBook.Category = bookVM.Book.Category;
                    oldVersionBook.ImageURL = bookVM.Book.ImageURL;
                    _db.Books.Update(oldVersionBook);
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
                var bookOnDelete = await _db.Books.FindAsync(id);
                return View(bookOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Book book)
        {
            var bookOnDelete = await _db.Books.FindAsync(book.BookId);
            if (bookOnDelete != null)
            {
                _db.Books.Remove(bookOnDelete);
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

