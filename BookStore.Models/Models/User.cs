using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Можно вводить только буквы.")]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public string? DateofBirth { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Ctreet { get; set; }
        public int HouseNumber { get; set; }
        [DataType(DataType.PhoneNumber)]
        [Required]
        public string? PhoneNumber { get; set; }
        
        public int PersonalWallet { get; set; } 
    }
}
