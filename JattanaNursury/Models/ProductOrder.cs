using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JattanaNursury.Models
{
    public class ProductOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set;  }
    }
}
