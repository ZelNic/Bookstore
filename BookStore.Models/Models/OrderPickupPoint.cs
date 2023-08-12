using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class OrderPickupPoint
    {
        [Key] public int PointId { get; set; }
        [Required] public string Region { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Street { get; set; }
        [Required] public string BuildingNumber { get; set; }
        [Required][DataType(DataType.DateTime)] public string WorkingHours { get; set; }
    }
}
