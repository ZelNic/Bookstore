using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class ShoppingBasket
    {
        [Key] public Guid BasketId { get; set; }
        [Required] public Guid UserId { get; set; }
        [Required] public string? ProductIdAndCount { get; set; }
    }
}
