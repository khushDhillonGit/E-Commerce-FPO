using Microsoft.AspNetCore.Identity;

namespace JattanaNursury.Models
{
    public class ApplicationUser: IdentityUser<Guid>
    {

        public string? Name { get; set; }
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; }
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }

    public class ApplicationRole : IdentityRole<Guid> 
    {
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
