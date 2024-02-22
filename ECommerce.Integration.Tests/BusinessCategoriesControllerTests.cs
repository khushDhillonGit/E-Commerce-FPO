using ECommerce.Controllers;
using ECommerce.Data;
using ECommerce.Integration.Tests.Helpers;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Integration.Tests
{
    public class BusinessCategoriesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly UserManager<ApplicationUser> _userManager;
        public BusinessCategoriesControllerTests(CustomWebApplicationFactory<Program> factory) 
        {
            _factory = factory;
            _userManager = Utilities.MockUserManager().Object;
        }

        [Fact]
        public async Task Index_WithCategories_ReturnsAllBusinessCategories()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            var categories = Utilities.GetBusinessCategories();
            await _context.BusinessCategories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BusinessCategory>>(viewResult.Model);
            Assert.Equal(categories.Count, model.Count);
        }

        [Fact]
        public async Task Index_NullCategories_ReturnsProblem()
        {            
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();
            _context.BusinessCategories = null;
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ObjectResult>(result);
            Assert.NotNull(viewResult.Value);
            Assert.Equal("Entity set 'ApplicationDbContext.BusinessCategories'  is null.", ((ProblemDetails)viewResult.Value).Detail);
        }

    }
}