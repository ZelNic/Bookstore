using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Worker
    {
        [Key]
        public Guid WorkerId { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public Guid? OfficeId { get; set; }
        public string? OfficeName { get; set; }
        public string? Post { get; set; }
        public string? AccessRights { get; set; }
        public string? AdmissionOrder { get; set; }
        public string? OrderDismissal { get; set; }

    }
}
