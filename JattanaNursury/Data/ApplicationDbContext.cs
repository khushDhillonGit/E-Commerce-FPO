using JattanaNursury.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JattanaNursury.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
    }
}