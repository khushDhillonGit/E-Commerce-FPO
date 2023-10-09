using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JattanaNursury.Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<ProductOrder> ProductOrders { get; set; }
        [Required]
        public string EmployeeId { get; set; }

        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal BillPrice { get; set; }

        //Customer
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string FullAddress { get; set; }
        [DefaultValue("getutcdate()")]
        public DateTime OrderDate { get; set; }
    }
}
