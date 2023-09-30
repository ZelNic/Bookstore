using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Street { get;  set; }        
        [Required] public int ResponsiblePersonId { get; set; }
    }
}
