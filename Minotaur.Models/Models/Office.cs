using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Office
    {
        [Key]
        public Guid Id { get; set; }
        [Required] public string? Name { get; set; }
        [Required] public string? Type { get; set; }
        [Required] public string? Status { get; set; }
        [Required] public string? City { get; set; }
        [Required] public string? Street { get; set; }
        [Required] public string? BuildingNumber { get; set; }
        [Required] public string? WorkingHours { get; set; }
        [Required] public string? SupervisorId { get; set; }
        public string? Workload { get; set; }
        public string? Notes { get; set; }

    }
}
