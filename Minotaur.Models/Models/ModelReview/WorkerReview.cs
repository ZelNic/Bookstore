using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class WorkerReview
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid WorkerId { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? WorkerReviewText { get; set; }
        public string? StartingPosition { get; set; }
        public string? EndPosition { get; set; }
    }
}
