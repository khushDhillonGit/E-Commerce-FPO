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
            Orders = new List<Order>();
            Owners = new List<ApplicationUser>();
            Venders = new List<Vender>();
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
        public virtual Address Address { get; set; }    
        public Guid BusinessCategoryId { get; set; }
        [ForeignKey(nameof(BusinessCategoryId))]
        public virtual BusinessCategory BusinessCategory { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ApplicationUser> Owners { get; set; }
        public virtual ICollection<BusinessEmployee> Employees { get; set; }
        public virtual ICollection<Vender> Venders { get; set; }
        public string? ImageUrl { get; set; }  
        public bool IsDelete { get; set; }
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;
    }
}
