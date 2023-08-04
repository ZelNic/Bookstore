using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class WishList
    {
        [Key] public int WishListId { get; set; }
        [Required] public int UserId { get; set; }
        [Required] public int ProductId { get; set; }
        [Required] public int CountProduct { get; set; }
    }
}
