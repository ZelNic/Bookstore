using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models.ModelReview
{
    public class ProductReviews
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public Guid UserId { get; set; }
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
        [Range(1, 5)] public int Rating { get; set; }
        public string? Review { get; set; }
        public bool IsAnonymous { get; set; } = false;

    }
}
