using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.Models
{
    public class Employees
    {
        [Key]
        public int EmployeeId { get; set; }
        public string RoleName { get; set; }
        public string Id { get; set; }
    }
}
