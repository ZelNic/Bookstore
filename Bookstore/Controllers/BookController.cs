using Bookstore.Models;
using Microsoft.AspNetCore.Mvc;

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

            return View(book,);
        }

        [HttpPost]
        public IActionResult Upsert(Book book)
        {
            if(book.Id == 0)
            {
                _db.Add(book);
                
            }
            else
            {
                _db.Update(book);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
