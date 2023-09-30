using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class ShoppingBasket
    {
        [Key] public int BasketId { get; set; }
        [Required] public int UserId { get; set; }
        [Required] public string ProductIdAndCount { get; set; }
    }
}
