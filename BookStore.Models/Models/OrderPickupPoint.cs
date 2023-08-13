using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookstore.Models.Models
{
    public class OrderPickupPoint
    {
        [Key] public int PointId { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Street { get; set; }
        [Required] public string BuildingNumber { get; set; }
        [Required][DataType(DataType.DateTime)] public string WorkingHours { get; set; }
        [Required] public int CountOfOrders{ get; set; }
    }
}
