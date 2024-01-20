﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? CustomerName { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsDelete { get; set; }
    }
}
