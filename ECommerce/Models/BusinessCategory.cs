using Microsoft.Build.Framework;

namespace ECommerce.Models
{
    public class BusinessCategory
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
