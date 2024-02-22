using ECommerce.Controllers;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerce.Tests
{
    public class BusinessCategoriesControllerTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public BusinessCategoriesControllerTests()
        {
            _userManager = FakeData.MockUserManager().Object;
            _connection = new SqliteConnection("datasource=:memory:");
            _connection.Open();

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

        }

        [Fact]
        public async Task Index_WithCategories_ReturnsAllBusinessCategories()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();

            var categories = FakeData.GetBusinessCategories();
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
            using var _context = new ApplicationDbContext(_options);
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
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();

            BusinessCategory bc = new BusinessCategory() { Name = Guid.NewGuid().ToString(), Description = "Test Cat" };
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Create(bc);
            var savedBc = await _context.BusinessCategories.FirstOrDefaultAsync(a => a.Name == bc.Name);

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
            using var _context = new ApplicationDbContext(_options);
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
            Assert.Equal(false, controller?.ModelState.IsValid);
        }

        [Fact]
        public async Task Details_IdIsNull_ReturnsNotFound()
        {
            //Arrange
            var controller = new BusinessCategoriesController(_userManager, null!);

            //Act
            var result = await controller.Details(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Details_CategoryDoesnExist_ReturnsNotFound()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Details(It.IsAny<Guid>());

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Details_CategoryExists_ReturnsViewWithCategory()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Details(bc.Id);

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            Assert.NotNull(vr.Model);
            Assert.Equal(bc.Id, ((BusinessCategory)vr.Model).Id);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }


        [Fact]
        public async Task Edit_Get_IdIsNull_ReturnsNotFound()
        {
            //Arrange
            var controller = new BusinessCategoriesController(_userManager, null!);

            //Act
            var result = await controller.Edit(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Edit_Get_CategoryDoesnExist_ReturnsNotFound()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Edit(It.IsAny<Guid>());

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Edit_Get_CategoryExists_ReturnsViewWithCategory()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
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

        [Fact]
        public async Task Edit_Post_IdAndModelIdAreDifferent_ReturnsNotFound()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();

            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Edit(It.IsAny<Guid>(), bc);

            //Assert
            Assert.IsType<NotFoundResult>(result);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }


        [Fact]
        public async Task Edit_Post_ModelIsInvalid_ReturnsViewWithModel()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();

            var controller = new BusinessCategoriesController(_userManager, _context);
            controller.ModelState.AddModelError("Name", "Name is required");

            //Act
            var result = await controller.Edit(bc.Id, bc);

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BusinessCategory>(vr.Model);
            Assert.Equal(bc.Id, model.Id);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }

        [Fact]
        public async Task Edit_Post_ModelValid_UpdatesCategory()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();

            var controller = new BusinessCategoriesController(_userManager, _context);

            bc.Name = "New Name";
            //Act
            var result = await controller.Edit(bc.Id, bc);
            var newBc = await _context.BusinessCategories.FirstOrDefaultAsync(a => a.Id == bc.Id);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(newBc);
            Assert.Equal(bc.Name, newBc.Name);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }


        [Fact]
        public async Task Delete_Get_IdIsNull_ReturnsNotFound()
        {
            //Arrange
            var controller = new BusinessCategoriesController(_userManager, null!);

            //Act
            var result = await controller.Delete(null);

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Delete_Get_CategoryDoesnExist_ReturnsNotFound()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Delete(It.IsAny<Guid>());

            //Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Delete_Get_CategoryExists_ReturnsViewWithCategory()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureCreated();
            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.Delete(bc.Id);

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            Assert.NotNull(vr.Model);
            Assert.Equal(bc.Id, ((BusinessCategory)vr.Model).Id);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }


        [Fact]
        public async Task Delete_Post_CategoryExists_RemovedCategory()
        {
            //Arrange
            using var _context = new ApplicationDbContext(_options);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();


            var bc = new BusinessCategory() { Id = Guid.NewGuid(), Name = "Test Details", Description = "Test Details" };
            _context.BusinessCategories.Add(bc);
            await _context.SaveChangesAsync();
            var controller = new BusinessCategoriesController(_userManager, _context);

            //Act
            var result = await controller.DeleteConfirmed(bc.Id);
            var delBc = _context.BusinessCategories.FirstOrDefault(a => a.Id == bc.Id);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(delBc);
            Assert.True(delBc.IsDelete);

            //Cleanup
            await _context.Database.ExecuteSqlRawAsync("Delete from BusinessCategories;");
        }

    }
}