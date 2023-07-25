using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookstore.Models
{
    [Table("BooksTable")]
    
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Price { get; set; }        
        
        public string? ImageURL { get; set; }

        public int? Category { get; set; } // need add
    }
}
