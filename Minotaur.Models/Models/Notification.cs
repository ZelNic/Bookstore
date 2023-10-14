using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)][Required] public DateTime SendingTime { get; set; }
        public Guid OrderId { get; set; }
        [Required] public Guid SenderId { get; set; }
        [Required] public Guid RecipientId { get; set; }
        [Required] public string? Text { get; set; }
        [Required] public bool IsHidden { get; set; }
    }
}
