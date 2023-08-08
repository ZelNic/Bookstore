using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        public ProductData ProdData { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; }
        public int PurchaseAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string CurrentPosition { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string TravelHistory { get; set; } = string.Empty;
        public bool isCourierDelivery { get; set; } = false;
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public int HouseNumber { get; set; }
        [DataType(DataType.PhoneNumber)] public string? PhoneNumber { get; set; }
    }
}
