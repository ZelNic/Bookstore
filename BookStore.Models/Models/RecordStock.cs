using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class RecordStock
    {
        [Key]
        public int Id { get; set; }
        [Required] public int StockId { get; set; }
        [Required] public int ResponsiblePersonId { get; set; }
        [Required] public DateTime Time { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int ShelfNumber { get; set; }
        [Required] public string Operation { get; set; }
        public bool IsOrder { get; set; }
        public string? ProductDataOnPurchase { get; set; }
    }
}
