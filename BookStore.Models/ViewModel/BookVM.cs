using Bookstore.Models.Models;

namespace Bookstore.Models
{
    public class BookVM
    {
        public User? User { get; set; }
        public Book? Book { get; set; }
        public List<Book>? BooksList { get; set; }
        public List<Category>? CategoriesList { get; set; }
        public WishList? WishList { get; set; }
        public ShoppingBasketClient? ShoppingBasket { get; set; }
    }
}
