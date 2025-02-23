using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceApplication.BLL.DTO
{
    public class UserProfileUpdateDTO
    {
        [Required(ErrorMessage = "please provide first name"), MaxLength(100)]
        public string firstName { get; set; }

        [Required(ErrorMessage = "please provide last name"), MaxLength(100)]
        public string lastName { get; set; }

        [EmailAddress, Required, MaxLength(100)]
        public string email { get; set; }
        [MaxLength(20)]
        public string? phone { get; set; }

        [Required, MinLength(15), MaxLength(100)]
        public string address { get; set; }
    }
}
