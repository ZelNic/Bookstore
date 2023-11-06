using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class PickUpReview
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid OrderPickupPointId { get; set; }
        [Required][Range(1, 5)] public int PickUpRating { get; set; }
        public string? PickUpReviewText { get; set; }
    }
}
