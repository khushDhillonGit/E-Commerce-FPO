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

namespace ECommerce.Integration.Tests
{

    public class BusinessesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly ImageUtility _imageUtility;
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

    }
}
