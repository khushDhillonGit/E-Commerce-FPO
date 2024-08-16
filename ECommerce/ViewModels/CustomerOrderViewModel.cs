using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class CustomerOrderViewModel
    {
        public Guid OrderId { get; set; }
        public string? OrderDate { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal BillPrice { get; set; }
        public string? OrderNumber { get; set; }
        public string? BusinessName { get; set; }
        public List<CustomerOrderProductsModel> Products { get; set; } = new List<CustomerOrderProductsModel>();
    }

    public class CustomerOrderProductsModel
    {
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
