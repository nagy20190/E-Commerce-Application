using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceApplication.BLL.DTO
{
    public class CartDTO
    {
        public List<CartItemDTO> items { get; set; } = new List<CartItemDTO>();
        public decimal SubTotal { get; set; }
        public decimal ShipingFee { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
