using Bookstore.Models.Models;

namespace Bookstore.Models
{
    public class BookVM
    {
        public List<Book>? BooksList { get; set; }
        public List<Category>? CategoriesList { get; set; }
        public Book? Book { get; set; }
        public List<WishList>? WishList { get; set; }
    }
}
