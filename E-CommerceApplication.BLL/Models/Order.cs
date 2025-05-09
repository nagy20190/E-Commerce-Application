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
    public class Order : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        [Precision(16, 2)]
        public decimal ShippingFee { get; set; }

        [MaxLength(100)]
        public string DeliveryAddress { get; set; } = "";

        [MaxLength(30)]
        public string PaymentMethod { get; set; } = "";

        [MaxLength(30)]
        public string PaymentStatus { get; set; } = "";

        [MaxLength(30)]
        public string OrderStatus { get; set; } = "";

        // navigation properties
        public User User { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
