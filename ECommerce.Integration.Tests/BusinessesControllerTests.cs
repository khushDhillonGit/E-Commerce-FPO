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
using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using System.Linq;
using NuGet.Protocol;
using Newtonsoft.Json;
using Moq;
using ECommerce.Models.Api;

namespace ECommerce.Integration.Tests
{

    public class BusinessesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ImageUtility _imageUtility;
        private readonly string imageSavePath = Path.Combine("images", "businesses");
        private readonly Mapper _mapper;

        public BusinessesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _imageUtility = _factory.Services.GetRequiredService<ImageUtility>();
            _mapper = new Mapper(new MapperConfiguration(a => 
            { 
                a.CreateMap<BusinessViewModel, Business>().ReverseMap();
                a.CreateMap<Address,AddressViewModel>().ReverseMap();
            }));
        }

        [Fact]
        public async Task Index_WithCurrentUser_ReturnsCurrentUserBusinesses()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await _context.SeedDbWithUsersAndBusinesses(_userManager);

            var controller = new BusinessesController(_imageUtility, _userManager, _context);
            var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
            if (bo == null) throw new NullReferenceException("User is null, please check Id and make sure you are seeding database");
            controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

            //Act
            var result = await controller.Index();

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<List<BusinessViewModel>>(vr.Model);
            Assert.Equal(bo.Businesses.Count, vm.Count);
            Assert.Equal(bo.Businesses.SelectMany(a => a.ProductCategories).SelectMany(a => a.Products).Count(), vm.Sum(a => a.TotalProducts));
            Assert.Equal(bo.Businesses.SelectMany(a => a.Employees).Count(), vm.Sum(a => a.TotalEmployees));
            Assert.Equal(bo.Businesses.SelectMany(a => a.ProductCategories).Count(), vm.Sum(a => a.TotalCategories));
        }

        [Fact]
        public async Task CurrentBusiness_UnauthorisedUser_ReturnsUnauthorised()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await _context.SeedDbWithUsersAndBusinesses(_userManager);

            var controller = new BusinessesController(_imageUtility, _userManager, _context);
            //this user doesnt have any business registered
            var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("6D969212-285E-401A-8EA7-350E50179988"));
            if (bo == null) throw new NullReferenceException("User is null, please check Id and make sure you are seeding database");
            controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

            //Act
            var result = await controller.CurrentBusiness(Guid.Parse("4B9DA798-63E4-456E-9177-AEB61494C94A"));

            //Assert
            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public async Task CurrentBusiness_AuthorisedUser_ReturnsViewWithBusiness()
        {
            //Arrange
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await _context.SeedDbWithUsersAndBusinesses(_userManager);

            Guid bussId = Guid.Parse("4B9DA798-63E4-456E-9177-AEB61494C94A");
            var controller = new BusinessesController(_imageUtility, _userManager, _context);
            //this user doesnt have any business registered
            var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
            if (bo == null) throw new NullReferenceException("User is null, please check Id and make sure you are seeding database");
            controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

            var buss = _context.Businesses.FirstOrDefault(a=>a.Id == bussId);
            if (buss == null) throw new NullReferenceException("business is null, please check Id and make sure you are seeding database");
            //Act
            var result = await controller.CurrentBusiness(bussId);

            //Assert
            var vr = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<BusinessViewModel>(vr.Model);
            Assert.Equal(bussId, vm.Id);
            Assert.Equal(buss.ProductCategories.SelectMany(a => a.Products).Count(), vm.TotalProducts) ;
            Assert.Equal(buss.Employees.Count(), vm.TotalEmployees);
            Assert.Equal(buss.ProductCategories.Count(), vm.TotalCategories);
        }

        [Fact]
        public async Task Create_ModelStateInvalid_ReturnsViewWithCategories() 
        {
            using var scope = _factory.Services.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
            await _context.SeedDbWithUsersAndBusinesses(_userManager);

            var controller = new BusinessesController(_imageUtility,_userManager,_context);
            BusinessViewModel model = new();
            controller.ControllerContext.ModelState.AddModelError("Name","Name is required");

            var result = await controller.Create(model); 
            
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsType<BusinessViewModel>(viewResult.Model);
            Assert.NotNull(modelResult.Categories);
            dynamic itemList = JsonConvert.DeserializeObject(modelResult.Categories.Items.ToJson()) ?? new List<BusinessCategory>();
            Guid catId = itemList?[0].Id;
            Assert.NotNull(_context.BusinessCategories.FirstOrDefault(a=>a.Id == catId));
        }


		[Fact]
		public async Task Create_ModelStateValid_CreatesBusiness()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName,ApplicationRole.BusinessOwner);
			BusinessViewModel model = new() 
            {
                Name = "Arora Mobile",
                Description = "We sell used mobiles",
                Phone = "9999999999",
                BusinessCategoryId = _context.BusinessCategories.FirstOrDefault().Id
            };

            model.Address = new AddressViewModel() { StreetAddress = "123 Test Street", City = "Edmonton", Country = "Canada", PostalCode = "L4L 4L4", Province = "ON"};

			var result = await controller.Create(model);

			Assert.IsType<RedirectToActionResult>(result);
            var newBusiness = await _context.Businesses.FirstOrDefaultAsync(a => a.Id == model.Id);
			Assert.NotNull(newBusiness);
            Assert.Equal(model.BusinessCategoryId,newBusiness.BusinessCategoryId);
            Assert.Equal(model.Name,newBusiness.Name);
            Assert.Equal(model.Address.StreetAddress,newBusiness.Address.StreetAddress);
		}

        [Fact]
        public async Task EditGet_BusinessNotFound_ReturnsNotFound() 
        {
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

            var result = await controller.Edit(It.IsAny<Guid>());

            Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task EditGet_BusinessFound_ReturnsViewWithModel()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);
            var business = await _context.Businesses.FirstOrDefaultAsync();

			var result = await controller.Edit(business.Id);

			var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BusinessViewModel>(viewResult.Model);
            Assert.Equal(business.Id,model.Id);
		}

		[Fact]
		public async Task EditPost_ModelStateInvalid_ReturnsViewWithModel()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);
            var business = _mapper.Map<BusinessViewModel>(await _context.Businesses.FirstOrDefaultAsync());
            controller.ControllerContext.ModelState.AddModelError("Name","");

			var result = await controller.Edit(business.Id,business);

			var viewResult = Assert.IsType<ViewResult>(result);
			var modelResult = Assert.IsType<BusinessViewModel>(viewResult.Model);
			Assert.NotNull(modelResult.Categories);
			dynamic itemList = JsonConvert.DeserializeObject(modelResult.Categories.Items.ToJson()) ?? new List<BusinessCategory>();
			Guid catId = itemList?[0].Id;
			Assert.NotNull(_context.BusinessCategories.FirstOrDefault(a => a.Id == catId));
		}

		[Fact]
		public async Task EditPost_ModelStateValid_EditsAndSavesBusiness()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);
			var business = _mapper.Map<BusinessViewModel>(await _context.Businesses.FirstOrDefaultAsync());
            business.Name = "New Name";

			var result = await controller.Edit(business.Id, business);

			Assert.IsType<RedirectToActionResult>(result);
			var newBusiness = await _context.Businesses.FirstOrDefaultAsync(a => a.Id == business.Id);
			Assert.NotNull(newBusiness);
			Assert.Equal(business.BusinessCategoryId, newBusiness.BusinessCategoryId);
			Assert.Equal(business.Name, newBusiness.Name);
			Assert.Equal(business.Address.StreetAddress, newBusiness.Address.StreetAddress);
		}

        [Fact]
        public async Task Delete_IdNull_ReturnsNotFound() 
        {
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

            var result = await controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_BusinessNotFound_ReturnsNotFound()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);

			var result = await controller.Delete(It.IsAny<Guid>());

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_BusinessFound_DeletesBusinessReturnsOkSucess()
		{
			using var scope = _factory.Services.CreateScope();
			var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var _imageUtility = scope.ServiceProvider.GetRequiredService<ImageUtility>();
			await _context.SeedDbWithUsersAndBusinesses(_userManager);

			Guid bussId = Guid.Parse("4B9DA798-63E4-456E-9177-AEB61494C94A");
			var controller = new BusinessesController(_imageUtility, _userManager, _context);
			var bo = _context.Users.FirstOrDefault(a => a.Id == Guid.Parse("2CFD97F6-8136-4FCD-BBC5-5F2B72539B42"));
			controller.SetUserHttpContext(bo.UserName, ApplicationRole.BusinessOwner);
			var buss = await _context.Businesses.FirstOrDefaultAsync(a => a.Id == bussId);
			var prod = _context.Categories.Where(a => a.BusinessId == bussId).Select(a => a.Products);
			Assert.NotNull(buss);
			Assert.NotEqual(0, prod.Count());

			var result = await controller.Delete(bussId);

            var bussDel = await _context.Businesses.FirstOrDefaultAsync(a=>a.Id == bussId);
            var products = _context.Categories.Where(a => a.BusinessId == bussId).Select(a => a.Products);
			Assert.Null(bussDel);
            Assert.Equal(0,products.Count());
            dynamic res = Assert.IsType<OkObjectResult>(result).Value ?? new PostBackModel();
            Assert.True(res.Success);
		}

	}
}
