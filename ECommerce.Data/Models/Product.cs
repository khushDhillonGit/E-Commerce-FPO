using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data.Models
{
    public class Product
    {
        public Product() 
        {
            Name = string.Empty;
            Description = string.Empty; 
        }
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } 
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string? ImageUrl { get; set; }
        public string? SKU { get; set; }
        public decimal Quantity { get; set; }
        public string? Test { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; } 
        public bool IsDelete { get; set; }
        public virtual ICollection<ProductHistory> ProductHistories { get; set; } = new List<ProductHistory>();
    }
}
