using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Models.Models
{
    public class ShoppingBasket
    {
        [Key] public int BasketId { get; set; }
        [Required][ForeignKey("UserId")] public int UserId { get; set; }
        [Required][ForeignKey("BookId")] public int ProductId { get; set; }
        [Required] public int CountProduct { get;  set; }

    }
}
