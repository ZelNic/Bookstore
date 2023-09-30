using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.DateTime)][Required] public DateTime SendingTime { get; set; }
        public int OrderId { get; set; }
        [Required] public int SenderId { get; set; }
        [Required] public int RecipientId { get; set; }
        [Required] public string Text { get; set; }
        [Required] public bool IsHidden { get; set; }
    }
}
