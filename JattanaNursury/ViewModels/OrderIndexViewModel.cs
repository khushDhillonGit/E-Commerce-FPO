namespace JattanaNursury.ViewModels
{
    public class OrderIndexViewModel
    {
        public Guid OrderId { get; set;}
        public string? OrderDate { get; set;}
        public decimal Price { get; set;}
        public decimal Discount { get; set;}
        public decimal BillPrice { get; set;}
        public decimal PaidByCustomer { get; set;}
        public string? Employee { get; set; }
        public string? OrderNumber { get; set;}
    }
}
