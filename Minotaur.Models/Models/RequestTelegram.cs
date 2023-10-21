using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class RequestTelegram
    {
        [Key]
        public Guid Id { get; set; }
        public long ChatId { get; set; }
        public string? Operation { get; set; }
    }
}
