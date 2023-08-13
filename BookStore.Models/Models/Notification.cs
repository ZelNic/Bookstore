using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [DataType(DataType.DateTime)][Required] public DateTime SendingTime { get; set; }
        [Required] public int SenderId { get; set; }
        [Required] public int RecipientId { get; set; }
        [Required] public string Text { get; set; }
    }
}
