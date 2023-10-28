using Minotaur.Models.Models;

namespace Minotaur.Models
{
    public class ProductVM
    {
        public string? User { get; set; }
        public Product? Product { get; set; }
        public IEnumerable<Product>? ProductsList { get; set; }
        public IEnumerable<Category>? CategoriesList { get; set; }
        public WishList? WishList { get; set; }
        public ShoppingBasketClient? ShoppingBasket { get; set; }
    }
}
