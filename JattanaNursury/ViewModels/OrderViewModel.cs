using JattanaNursury.Models;

namespace JattanaNursury.ViewModels
{
    public class OrderViewModel
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

    }
    public class ProductOrderDetail 
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
