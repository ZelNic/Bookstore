﻿using System.ComponentModel.DataAnnotations;

namespace Minotaur.Models.OrganizationalDocumentation.HR
{
    public class OrganizationalOrder
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Date { get; set; }
    }
}
