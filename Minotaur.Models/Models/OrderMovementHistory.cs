using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class OrderMovementHistory
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string? CurrentPosition { get; set; }
        public string? HistoryOfСonversion { get; set; }
        public DateTime DispatchTime { get; set; }
        public DateTime TimeOfReceiving { get; set; }
    }
}
