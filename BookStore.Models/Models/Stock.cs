using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required] public string City { get; set; }
        [Required] public string Street { get;  set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int ShelfNumber { get; set; }
        [Required] public int ResponsiblePerson { get; set; }
    }
}
