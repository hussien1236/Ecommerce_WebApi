﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApplication9.Models
{
    [Index("Email",IsUnique=true)]
    public class ResetRequests
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string Token { get; set; }="";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
