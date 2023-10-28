using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Worker
    {
        [Key]
        public Guid WorkerId { get; set; }
        public Guid UserId { get; set; }
        public Guid OfficeId { get; set; }
        public string? Status { get; set; }
        public string? OfficeName { get; set; }
        public string? Post { get; set; }
        public string? AccessRights { get; set; }
        public int AdmissionOrder { get; set; }
        public int OrderDismissal { get; set; }
        public int WorkerRating { get; set; }
    }
}
