using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class BaseOrderReviews
    {
        public Guid OrderId { get; set; }
        [Required][Range(1, 5)] public int DeliveryRating { get; set; }
        public string? DeliveryReviewText { get; set; }
        [Required][Range(1, 5)] public int PickUpRating { get; set; }
        public string? PickUpReviewText { get; set; }
        [Required][Range(1, 5)] public int WorkerRating { get; set; }
        public string? WorkerReviewText { get; set; }
    }
}
