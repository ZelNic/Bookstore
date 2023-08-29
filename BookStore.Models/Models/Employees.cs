﻿using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models.Models
{
    public class Employees : User
    {
        [Key]
        public int EmployeeId { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
    }
}
