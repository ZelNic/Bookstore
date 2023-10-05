using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Office
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? WorkingHours { get; set; }
        public string? SupervisorId { get; set; }
        public string? Workload { get; set; }
        public string? Notes { get; set; }  

    }
}
