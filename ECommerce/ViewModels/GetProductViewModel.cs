namespace ECommerce.ViewModels
{
    public class GetProductViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
