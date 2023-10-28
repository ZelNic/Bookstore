using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class WorkerRating
    {
        [Key]
        public Guid Id { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? WorkerReviewText { get; set; }
    }
}
