using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class WishList
    {
        [Key] public int WishListId { get; set; }
        [Required] public string UserId { get; set; }
        [Required] public string? ProductId { get; set; }
    }
}
