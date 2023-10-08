using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Worker
    {
        [Key]
        public Guid WorkerId { get; set; }
        [Required] public string? Status { get; set; }
        [Required] public string? UserId { get; set; }
        [Required] public string? OfficeId { get; set; }
        [Required] public string? Role { get; set; }
        public string? AdmissionOrder { get; set; }
        public string? OrderDismissal { get; set; }
        
    }
}
