namespace ECommerce.ViewModels
{
    public class ProductViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTimeOffset CreatedDate { get; set; }
    }
}
