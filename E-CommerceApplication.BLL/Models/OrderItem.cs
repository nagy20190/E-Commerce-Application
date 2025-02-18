using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_CommerceApplication.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApplication.BLL.Models
{
    public class OrderItem : IEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }
        // navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
