﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data.Models
{
    public class Order
    {
        public Order()
        {
            OrderProducts = new List<ProductOrder<Order>>();
            OrderNumber = string.Empty;
        }

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual ICollection<ProductOrder<Order>> OrderProducts { get; set; }
        [Required]
        public string OrderNumber { get; set; }
        [Required]
        public string? EmployeeId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public decimal Price { get ; set ; }
        public decimal Discount { get; set; }

        public decimal BillPrice { get ; set ; }

        public bool FullyPaid { get; set; } = false;

        public decimal PaidByCustomer { get; set; }

        public DateTimeOffset OrderDate { get; set; }
        public Guid BusinessId { get; set; }
        [ForeignKey(nameof(BusinessId))]
        public virtual Business? Business { get; set; }

    }
}
