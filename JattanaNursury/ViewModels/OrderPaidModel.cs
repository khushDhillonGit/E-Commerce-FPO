namespace JattanaNursury.ViewModels
{
    public class OrderPaidModel
    {
        public Guid OrderId { get; set;}
        public string? OrderDate { get; set;}
        public decimal Price { get; set;}
        public decimal Discount { get; set;}
        public decimal BillPrice { get; set;}
        public string? Employee { get; set; }
        public string? OrderNumber { get; set;}
    }

    public class OrderUnpaidModel : OrderPaidModel
    {
        public decimal PaidByCustomer { get; set;}
    }

}
