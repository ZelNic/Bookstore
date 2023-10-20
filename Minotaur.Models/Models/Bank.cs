using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Bank
    {
        [Key]
        public Guid Id { get; set; }
        public int Sum { get; set; }
    }
}
