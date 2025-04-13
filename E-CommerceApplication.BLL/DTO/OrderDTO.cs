using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceApplication.BLL.DTO
{
   public class OrderDTO
    {
        [Required]
        public string ProductIdentifiers { get; set; }
        [Required, MinLength(10), MaxLength(150)]
        public string DeliveryAddress { get; set; }
        public string PaymentMethod { get; set; }
    }
}
