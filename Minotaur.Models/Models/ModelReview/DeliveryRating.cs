using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minotaur.Models.Models.ModelReview
{
    public class DeliveryRating
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? DeliveryReviewText { get; set; }
    }
}
