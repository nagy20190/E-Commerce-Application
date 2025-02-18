using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.BLL.DTO
{
    public class ProductDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required, MaxLength(100)]
        public string Category { get; set; } = "";
        [Required]
        public decimal Price { get; set; }
        [MaxLength(100)]
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; } 
    }
}
