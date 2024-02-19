using ECommerce.Controllers;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
namespace ECommerce.Tests
{
    public class BusinessCategoriesControllerTests
    {
        // Naming of Methods - UnitOfWork_StateUnderTest_Expected()

        private readonly Mock<ApplicationDbContext> _context;
        private readonly Mock<UserManager<ApplicationUser>> _userManager;

        public BusinessCategoriesControllerTests() 
        {
            _context = new Mock<ApplicationDbContext>();
            _userManager = FakeData.MockUserManager();
        }

        [Fact]
        public async void Index_WithCategories_ReturnsAllBusinessCategories()
        {
            //Arrange
            var categories = FakeData.GetBusinessCategories().AsQueryable();

            var mockSet = new Mock<DbSet<BusinessCategory>>();
            mockSet.As<IQueryable<BusinessCategory>>().Setup(m => m.Provider).Returns(categories.Provider);
            mockSet.As<IQueryable<BusinessCategory>>().Setup(m => m.Expression).Returns(categories.Expression);
            mockSet.As<IQueryable<BusinessCategory>>().Setup(m => m.ElementType).Returns(categories.ElementType);
            mockSet.As<IQueryable<BusinessCategory>>().Setup(m => m.GetEnumerator()).Returns(() => categories.GetEnumerator());

            _context.Setup(a=> a.BusinessCategories).Returns(mockSet.Object);

            var controller = new BusinessCategoriesController(_userManager.Object,_context.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BusinessCategory>>(viewResult.Model);
            Assert.Equal(categories.Count(),model.Count);
        }
    }
}