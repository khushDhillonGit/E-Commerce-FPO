using ECommerce.Controllers;
using ECommerce.Data;
using ECommerce.Integration.Tests.Helpers;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

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
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");


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

        [Fact]
        public async Task Create_ValidModel_CreateNewCategory() 
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            BusinessCategory bc = new BusinessCategory() { Name = Guid.NewGuid().ToString() , Description = "Test Cat"};
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Create(bc);
            var savedBc = await _context.BusinessCategories.FirstOrDefaultAsync(a=>a.Name == bc.Name);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(savedBc);
            Assert.Equal(bc.Name, savedBc.Name);
            Assert.Equal(bc.Description, savedBc.Description);

            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsViewWithModel()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();

            BusinessCategory bc = new BusinessCategory() { Description = "Test Cat" };
            var controller = new BusinessCategoriesController(_userManager, _context);
            controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var result = await controller.Create(bc);

            //Assert
            var savedBc = await _context.BusinessCategories.FirstOrDefaultAsync(a => a.Name == bc.Name);
            Assert.Null(savedBc);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(false,controller?.ModelState.IsValid);
        }

        [Fact]
        public async Task Details_IdIsNull_ReturnsNotFound()
        {
            //Arrange
            var controller = new BusinessCategoriesController(_userManager, null!);

            //Act
            var result = await controller.Edit(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Details_CategoryDoesnExist_ReturnsNotFound()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Edit(It.IsAny<Guid>());

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Details_CategoryExists_ReturnsViewWithCategory()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() {Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Edit(bc.Id);

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            Assert.NotNull(vr.Model);
            Assert.Equal(bc.Id, ((BusinessCategory)vr.Model).Id);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }



    }
}