using Org.BouncyCastle.Utilities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Business
    {
        public Business() 
        {
            Categories = new List<Category>();
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Phone { get; set; }
        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; }    
        public Guid BusinessCategoryId { get; set; }
        [ForeignKey(nameof(BusinessCategoryId))]
        public BusinessCategory BusinessCategory { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public string? ImageUrl { get; set; }  
        public bool IsDelete { get; set; }
        public DateTimeOffset CreatedDate { get; set; }    
    }
}
