using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class PurchaseRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]public DateTime ApplicationTime { get; set; }
        [Required] public int ResponsiblePersonId { get; set; }
        [Required] public string? ProductData { get; set; }
    }
}
