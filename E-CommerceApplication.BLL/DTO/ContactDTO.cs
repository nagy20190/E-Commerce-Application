using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;

namespace E_CommerceApplication.BLL.DTO
{
    public class ContactDTO
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string? Phone { get; set; }
        //public int subjectId { get; set; } 
        [Required, MinLength(20), MaxLength(4000)]
        public string Message { get; set; } = "";
        public Subject subject { get; set; }
    }
}
