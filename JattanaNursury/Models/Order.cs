using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JattanaNursury.Models
{
    public class Order
    {
        public Order()
        {
            ProductOrders = new List<ProductOrder>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<ProductOrder>? ProductOrders { get; set; }

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        public decimal Price { get ; set ; }
        public decimal Discount { get; set; }

        public decimal BillPrice { get ; set ; }

        [DefaultValue("getutcdate()")]
        public DateTime OrderDate { get; set; }

        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer? Customer { get; set; }

    }
}
