using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models
{
    public class User : IdentityUser
    {
        
        [Required]
        [RegularExpression("[a-zA-Zа-яА-Я]+", ErrorMessage = "Только буквенное обозначение")]
        public string? FirstName { get; set; }
        [Required]
        [RegularExpression("[a-zA-Zа-яА-Я]+", ErrorMessage = "Только буквенное обозначение")]
        public string? LastName { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public string? DateofBirth { get; set; } 
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }        
        public int PersonalWallet { get; set; }
    }
}
