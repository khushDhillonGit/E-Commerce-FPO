using JattanaNursury.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JattanaNursury.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,Guid,  ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,  ApplicationRoleClaim, ApplicationUserToken>
    {
        public virtual DbSet<Customer> Customers { get; set; }  
        public virtual DbSet<Category> Categories { get; set; }  
        public virtual DbSet<Order> Orders { get; set; }  
        public virtual DbSet<Product> Products { get; set; }  
        public virtual DbSet<ProductOrder> ProductOrders { get; set; }  

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasQueryFilter(a => !a.IsDelete);
            modelBuilder.Entity<Product>().HasQueryFilter(a => !a.IsDelete);
            modelBuilder.Entity<Customer>().HasQueryFilter(a => !a.IsDelete);
            modelBuilder.Entity<Vender>().HasQueryFilter(a => !a.IsDelete);

            modelBuilder.Entity<ApplicationUser>(b =>
            {

                b.HasQueryFilter(a=>!a.IsDelete);
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                // Each User can have many UserLogins
                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                // Each User can have many UserTokens
                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

            });

            modelBuilder.Entity<ApplicationRole>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();

            });
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            DeleteHandler();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            DeleteHandler();
            return base.SaveChanges();
        }

        private void DeleteHandler() 
        {
            var entities = ChangeTracker.Entries().Where(a => a.State == EntityState.Deleted);
            foreach (var entity in entities) 
            {
                var prop = entity.Entity.GetType().GetProperty("IsDelete");
                if (prop != null) 
                {
                    entity.State = EntityState.Modified;
                    prop.SetValue(entity.Entity, true, null);
                }
            }
        }
    }
}