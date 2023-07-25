using Bookstore.DataAccess;
using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _db;

        public BookController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Book> booksList = _db.Books.ToList();

            return View(booksList);
        }

        public IActionResult Upsert(int? bookId)
        {
            if (bookId == 0 || bookId == null)
            {
                Book newBook = new Book();

                return View(newBook);
            }

            var book = _db.Books.Find(bookId);

            return View(book);
        }

        [HttpPost]
        public IActionResult Upsert(Book book)
        {
            if (book.Id == 0)
            {
                _db.Add(book);

            }
            else
            {
                _db.Books.Update(book);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
               
        [HttpGet]
        [ActionName("Delete")]
        public IActionResult ConfirmDelete(int? id)
        {
            if (id != null)
            {
                var bookOnDelete = _db.Books.Find(id);
                return View(bookOnDelete);
            }
            else return NotFound();
        }

        [HttpPost]
        public IActionResult Delete(Book book)
        {
            var bookOnDelete = _db.Books.Find(book.Id);
            if (bookOnDelete != null)
            {
                _db.Books.Remove(bookOnDelete);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {                
                return NotFound();
            }
        }
    }
}
