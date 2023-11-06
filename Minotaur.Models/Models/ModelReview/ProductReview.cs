using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class ProductReview
    {
        [Key] public Guid Id { get; set; }
        [Required] public Guid OrderId { get; set; }
        [Required] public int ProductId { get; set; }
        [Required] public Guid UserId { get; set; }
        public string? FilePaths { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? ProductReviewText { get; set; }
        [Required] public DateTime PurchaseDate { get; set; }
        public bool IsShowReview { get; set; } = false;
        public bool IsAnonymous { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public string? IdWhoLiked { get; set; }
        public string? IdWhoDisiked { get; set; }
    }
}
