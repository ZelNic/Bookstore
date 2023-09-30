using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required,DataType(DataType.Text)]
        public string Name { get; set; }        
    }
}
