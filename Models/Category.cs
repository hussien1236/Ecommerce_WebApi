﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";   
    }
}
