using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class DeliveryReview
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? DeliveryReviewText { get; set; }
    }
}
