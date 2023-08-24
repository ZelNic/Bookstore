using Bookstore.Models.Models;

namespace Bookstore.Models
{
    public class StockVM
    {
        public IEnumerable<Stock>? Stock { get; set; }
        public IEnumerable<Book>? Books { get; set; }
    }
}
