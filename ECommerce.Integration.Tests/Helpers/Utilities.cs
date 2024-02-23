using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Integration.Tests.Helpers
{
    public static class Utilities
    {
        public static List<BusinessCategory> GetBusinessCategories()
        {
            List<BusinessCategory> categories = new List<BusinessCategory>();

            categories.Add(new BusinessCategory()
            {
                Id = Guid.Parse("DFBC6ABC-DFBD-470C-A225-BBB3E051EA6A"),
                Name = "Retail",
                Description = "Retail description"
            });

            categories.Add(new BusinessCategory()
            {
                Id = Guid.Parse("3C02BBC1-4ACF-411C-9FCA-B141833E0CE7"),
                Name = "Retail",
                Description = "Retail description"
            });

            return categories;
        }

        public static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var userManager = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            return userManager;
        }

        public static async Task SeedDbWithUsersAndBusinesses(this ApplicationDbContext? _context, UserManager<ApplicationUser> _userManager)
        {
            if (_context == null) return;

            _context.BusinessCategories.Add(new BusinessCategory() { Id = Guid.Parse("9C6E1E9B-1CB2-4D9F-8B6C-47B4A8584EAD"), Name = "Retail", Description = "Retail shops", CreatedDate = DateTimeOffset.UtcNow });
            _context.BusinessCategories.Add(new BusinessCategory() { Id = Guid.Parse("01EA3BA8-CDAF-4927-89EA-3CF7C7C8B5E5"), Name = "Health", Description = "Medical shops", CreatedDate = DateTimeOffset.UtcNow });
            _context.Roles.Add(new ApplicationRole() { Name = ApplicationRole.SuperAdmin,NormalizedName = ApplicationRole.SuperAdmin.ToUpper()});
            _context.Roles.Add(new ApplicationRole() { Name = ApplicationRole.BusinessOwner,NormalizedName = ApplicationRole.BusinessOwner.ToUpper()});

            await _context.SaveChangesAsync();


            Address address = new Address()
            {
                Id = Guid.NewGuid(),
                City = "Barrie",
                Province = "ON",
                PostalCode = "L4L 4L4",
                Country = "Canada",
                StreetAddress = "123 Test Street",
                UnitApt = "12",
                CreatedDate = DateTimeOffset.UtcNow
            };

            ApplicationUser admin = new()
            {
                Id = Guid.Parse("D8AD0A47-92D6-4B9D-A1E8-7C21C3BDE13A"),
                Email = "khushadmin@test.ca",
                EmailConfirmed = true,
                Name = "Khush Admin",
                NormalizedEmail = "khushadmin@test.ca".ToUpper(),
                UserName = "khushadmin@test.ca",
                NormalizedUserName = "khushadmin@test.ca".ToUpper(),
                PhoneNumber = "7057225555",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            };

            admin.Address = address;

            await _userManager.CreateAsync(admin, "Test@1234");
            await _userManager.AddToRoleAsync(admin, ApplicationRole.SuperAdmin);

            ApplicationUser bo = new()
            {
                Id = Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"),
                Email = "bo@test.ca",
                EmailConfirmed = true,
                NormalizedEmail = "bo@test.ca".ToUpper(),
                UserName = "bo@test.ca",
                Name = "Business Owner",
                NormalizedUserName = "bo@test.ca".ToUpper(),
                PhoneNumber = "7057225555",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            };
            bo.Address = address;

            var walmart = new Business()
            {
                Id = Guid.Parse("4B9DA798-63E4-456E-9177-AEB61494C94A"),
                Name = "Walmart",
                CreatedDate = DateTimeOffset.UtcNow,
                Phone = "9875556456",
                Address = address,
                Description = "Shop Groceries",
                BusinessCategoryId = Guid.Parse("9C6E1E9B-1CB2-4D9F-8B6C-47B4A8584EAD")
            };

            var specs = new Business()
            {
                Id = Guid.Parse("09BB0014-0310-4578-BA92-A004BA92E9E6"),
                Name = "Specsavers",
                CreatedDate = DateTimeOffset.UtcNow,
                Phone = "9875556456",
                Description = "Shop Specs",
                Address = address,
                BusinessCategoryId = Guid.Parse("9C6E1E9B-1CB2-4D9F-8B6C-47B4A8584EAD")
            };

            bo.Businesses.Add(walmart);
            bo.Businesses.Add(specs);

            var cat1 = new Category()
            {
                Id = Guid.Parse("7A47CC6A-635D-451A-9A88-1A9D9F233889"),
                Name = "Utensils",
                Description = "Util Des",
                CreatedDate = DateTimeOffset.UtcNow,
            };

            var cat2 = new Category()
            {
                Id = Guid.Parse("143923C7-D87D-42BB-B73D-CBE9E0A45CF2"),
                Name = "Specs",
                Description = "Specs",
                CreatedDate = DateTimeOffset.UtcNow,
            };

            var pro1 = new Product()
            {
                Id = Guid.Parse("B826B960-332A-4947-A4B7-95F74EE10D5A"),
                Name = "pro1",
                Description = "pro1",
                CreatedDate = DateTimeOffset.UtcNow,
                UnitPrice = 10,
                SellingPrice = 20,
                Quantity = 100,
            };

            var pro2 = new Product()
            {
                Id = Guid.Parse("9CD842EA-B90B-4BE8-AB4F-2300C4A2B888"),
                Name = "pro2",
                Description = "pro2",
                CreatedDate = DateTimeOffset.UtcNow,
                UnitPrice = 10,
                SellingPrice = 20,
                Quantity = 200,
            };

            var pro3 = new Product()
            {
                Id = Guid.Parse("652DBAE5-A23C-447D-8531-80FD5E2A2DA1"),
                Name = "pro3",
                Description = "pro3",
                CreatedDate = DateTimeOffset.UtcNow,
                UnitPrice = 10,
                SellingPrice = 20,
                Quantity = 200,
            };

            var pro4 = new Product()
            {
                Id = Guid.Parse("CF35C83C-5ADF-4697-A352-A46EEF728819"),
                Name = "pro4",
                Description = "pro4",
                CreatedDate = DateTimeOffset.UtcNow,
                UnitPrice = 10,
                SellingPrice = 20,
                Quantity = 200,
            };

            cat1.Products.Add(pro1);
            cat1.Products.Add(pro2);

            cat2.Products.Add(pro3);
            cat2.Products.Add(pro4);

            walmart.ProductCategories.Add(cat1);
            specs.ProductCategories.Add(cat2);

            await _userManager.CreateAsync(bo, "Test@1234");
            await _userManager.AddToRoleAsync(bo, ApplicationRole.BusinessOwner);
        }
    }
}
