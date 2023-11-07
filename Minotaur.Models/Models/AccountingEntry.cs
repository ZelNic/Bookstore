using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Minotaur.Models.Models
{
    public class AccountingEntry
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime RecordingTime { get; set; }
        public int Sum { get; set; }
        public string? Info { get; set; }
    }
}
