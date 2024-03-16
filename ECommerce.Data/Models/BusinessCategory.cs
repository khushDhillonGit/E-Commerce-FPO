using System.ComponentModel.DataAnnotations;

namespace ECommerce.Data.Models
{
    public class BusinessCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
