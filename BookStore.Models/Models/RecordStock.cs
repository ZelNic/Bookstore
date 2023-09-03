using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class RecordStock
    {
        [Key]
        public int Id { get; set; }
        [Required] public int StockId { get; set; }
        [Required] public int ResponsiblePersonId { get; set; }
        [Required] public int ProductId { get; set; }
        [Required] public int Count { get; set; }
        [Required] public int ShelfNumber { get; set; }
        [Required] public string Operation { get; set; }
        [Required] public bool IsOrder { get; set; }
    }
}
