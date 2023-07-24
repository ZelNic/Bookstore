using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Models
{
    [Table("BooksTable")]
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? ISBN { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public int? Category { get; set; }
        public string? ImageURL { get; set; }
    }
}
