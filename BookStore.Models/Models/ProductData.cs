using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Models.Models
{
    public class ProductData
    {
        public int Product { get; set; }
        public int Price { get; set; }
        public int CountProduct { get; set; }
    }
}
