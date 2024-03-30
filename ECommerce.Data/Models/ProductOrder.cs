using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data.Models
{
    public class ProductOrder<T>
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        public Guid LinkId { get; set; }
        [ForeignKey(nameof(LinkId))]
        public virtual T? Link { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice
        {
            get
            {
                return Quantity * Product?.UnitPrice ?? 0;
            }
        }
    }
}
