using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Review
    {
        [Key] public int ReviewsId { get; set; }
        [Required] public int BookId { get; set; }
        [Required] public int UserId { get; set; }
        public string Comment { get; set; }
        public string ImageUrl { get; set; }
        [Required][Range(1, 10)] public int ProductRating { get; set; }
        [Required] public DateTime PurchaseDate { get; set; }
        public bool IsShowReview { get; set; } = true;
    }
}
