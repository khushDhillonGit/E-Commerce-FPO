﻿namespace ECommerce.ViewModels
{
    public class ProductOnSaleViewModel
    {
        public Guid Id { get; set; }    
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? BusinessName { get; set; }
        public string? CategoryName { get; set; }
        public string? ImageUrl { get; set; }
    }
}
