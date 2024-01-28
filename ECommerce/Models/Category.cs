using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Category
    {
        public Category() 
        {
            Products = new List<Product>();
        }
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid BusinessId { get; set; }
        [ForeignKey(nameof(BusinessId))]
        public virtual Business? Business { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public bool IsDelete { get; set; }
    }
}
