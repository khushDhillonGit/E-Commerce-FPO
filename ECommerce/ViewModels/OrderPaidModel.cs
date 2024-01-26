namespace ECommerce.ViewModels
{
    public class OrderPaidModel
    {
        public Guid OrderId { get; set; }
        public string? OrderDate { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal BillPrice { get; set; }
        public string? Employee { get; set; }
        public string? OrderNumber { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public List<OrderProductsModel>  Products { get; set; } = new List<OrderProductsModel>();
    }

    public class OrderProductsModel 
    {
        public Guid ProductId { get; set; }
        public decimal? SellingPrice { get; set; }
        public string? Name { get; set; }        
        public decimal? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class OrderUnpaidModel : OrderPaidModel
    {
        public decimal PaidByCustomer { get; set;}
    }

}
