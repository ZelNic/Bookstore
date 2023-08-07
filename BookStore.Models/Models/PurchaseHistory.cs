using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class PurchaseHistory
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }

        [Required]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        public string ProductCount { get; set; } = string.Empty;

        [Required]
        public string ProductPrice { get; set; } = string.Empty;

        [DataType(DataType.DateTime), Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public int PurchaseAmount { get; set; }
        [Required]
        public string OrderStatus { get; set; } = string.Empty;
        [Required]
        public string CurrentPosition { get; set; } = string.Empty;
        [Required]
        public string TravelHistory { get; set; } = string.Empty;
    }
}
