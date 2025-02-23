using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Models;

namespace E_CommerceApplication.BLL.DTO
{
    public class CartItemDTO
    {
        public Product product { get; set; } = new Product();
        public int Quantity { get; set; }
    }
}
