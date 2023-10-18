using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)] public DateTime SendingTime { get; set; }
        public Guid OrderId { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public string? TypeNotification { get; set; }
        public string? EmailSender { get; set; }
        public string? EmailRecipien { get; set; }
        public string? Text { get; set; }
        public bool IsHidden { get; set; }
    }
}
