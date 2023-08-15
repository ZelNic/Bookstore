using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public int UserId { get; set; }
        public string? ReceiverName { get; set; }
        public string? RecipientsLastName { get; set; }
        public string? ProductData { get; set; }
        [DataType(DataType.DateTime)] public DateTime PurchaseDate { get; set; }
        public int PurchaseAmount { get; set; }
        public string? OrderStatus { get; set; }
        public string? CurrentPosition { get; set; }
        public string? DeliveryAddress { get; set; } 
        public string? TravelHistory { get; set; } 
        public bool isCourierDelivery { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        [DataType(DataType.PhoneNumber)] public string? PhoneNumber { get; set; }
        public int ConfirmationСode { get; set; }
    }
}
