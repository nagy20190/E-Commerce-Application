using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;

namespace E_CommerceApplication.BLL.Models
{
    public class Contact: IEntity
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; } = "";

        [MaxLength(100)]
        public string LastName { get; set; } = "";

        [MaxLength(100)]
        public string Email { get; set; } = "";

        [MaxLength(100)]
        public string Phone { get; set; } = "";

        public required Subject Subject { get; set; }

        public string Message { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}
