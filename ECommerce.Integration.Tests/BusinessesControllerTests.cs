using AutoMapper;
using ECommerce.Data;
using ECommerce.Services;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Models;
using ECommerce.Integration.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using ECommerce.Controllers;
using Microsoft.AspNetCore.Mvc;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ECommerce.Integration.Tests
{
 
    public class BusinessesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ImageUtility _imageUtility;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string imageSavePath = Path.Combine("images", "businesses");

        public BusinessesControllerTests(CustomWebApplicationFactory<Program> factory) 
        {
            _factory = factory;
            _imageUtility = _factory.Services.GetRequiredService<ImageUtility>();
        }

        [Fact]
        public async Task Index_WithCurrentUser_ReturnsCurrentUserBusinesses() 
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _context.Database.EnsureCreated();
            await _context.SeedDbWithUsersAndBusinesses(_userManager);

            var controller = new BusinessesController(_imageUtility,_userManager,_context);
            var bo = _context.Users.FirstOrDefault(a=>a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
            var context = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, bo.UserName), new Claim(ClaimTypes.Role, ApplicationRole.BusinessOwner)
                }))
            };

            controller.ControllerContext.HttpContext = context;

            //Act
            var result = await controller.Index();

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<List<BusinessViewModel>>(vr.Model);
            Assert.NotNull(bo);
            Assert.Equal(bo.Businesses.Count, vm.Count);
            Assert.Equal(bo.Businesses.SelectMany(a=>a.ProductCategories).Select(a=>a.Products).Count(), vm.Sum(a=>a.TotalProducts));
        }

    }
}
