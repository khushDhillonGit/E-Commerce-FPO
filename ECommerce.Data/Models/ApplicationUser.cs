using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data.Models
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public ApplicationUser() 
        {
            Logins = new List<ApplicationUserLogin>();
            Tokens = new List<ApplicationUserToken>();
            UserRoles = new List<ApplicationUserRole>();
            Claims = new List<ApplicationUserClaim>();
            CustomerOrders = new List<CustomerOrder>();
        }
        public string? Name { get; set; }
        public bool IsDelete { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public virtual Address? Address { get; set; }
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
        public virtual ICollection<CustomerOrder> CustomerOrders { get; set; }
        public virtual ICollection<Business> Businesses { get; set;} = new List<Business>();
        public virtual BusinessEmployee? BusinessEmployee { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid> 
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string BusinessOwner = "Business Owner";
        public const string Customer = "Customer";
        public const string Employee = "Employee";

        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationRole Role { get; set; }
        public virtual ApplicationUser User { get; set; }    
    }

    public class ApplicationUserClaim : IdentityUserClaim<Guid> 
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<Guid> 
    {
        public virtual ApplicationRole Role { get; set; }
    }
    public class ApplicationUserToken : IdentityUserToken<Guid> 
    {
        public virtual ApplicationUser User { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<Guid> 
    {
        public virtual ApplicationUser User { get; set; }
    }
}
