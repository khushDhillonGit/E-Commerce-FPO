using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Microsoft.EntityFrameworkCore.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Tests
{
    public static class FakeData
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

        public static DbContextOptions<ApplicationDbContext> GetDbContextOptions()
        {
            var service = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("MyTestDatabase").UseInternalServiceProvider(service);
            return builder.Options;
        }
    }
}
