using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models
{
    public class MinotaurUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Surname { get; set; }
        public string? DateofBirth { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public int PersonalWallet { get; set; }
    }
}
