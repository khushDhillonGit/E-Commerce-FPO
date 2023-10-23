using JattanaNursury.Models;

namespace JattanaNursury.ViewModels
{
    public class OrderViewModel
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullAddress { get; set; }
        public string EmailAddress { get; set; }
        public decimal Discount { get; set; }
        public string Employee { get; set; }
        public List<ProductOrderDetail> Products { get; set; } = new List<ProductOrderDetail>();
    }
    public class ProductOrderDetail 
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
