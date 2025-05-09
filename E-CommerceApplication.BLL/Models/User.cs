﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.BLL.Models
{
    [Index("Email", IsUnique = true)]
    public class User : IEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = ""; // unique in the database

        [MaxLength(20)]
        public string Phone { get; set; } = "";

        [MaxLength(100)]
        public string Address { get; set; } = "";

        [MaxLength(100)]
        public string Password { get; set; } = "";

        [MaxLength(20)]
        public string Role { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
