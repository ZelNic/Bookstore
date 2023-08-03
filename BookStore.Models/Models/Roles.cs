using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
    }
}
