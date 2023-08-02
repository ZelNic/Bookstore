using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class PurchaseHistory
    {
        [Key]
        public int PurchaseId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime PurchaseDate { get; set; }        
    }
}
