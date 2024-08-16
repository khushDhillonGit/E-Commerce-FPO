using AutoMapper;
using ECommerce.Data.Models;
using ECommerce.Services;
using ECommerce.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Integration.Tests
{
    public class EmployeesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly Mapper _mapper;
        private readonly ImageUtility _imageUtility;
        public EmployeesControllerTests(CustomWebApplicationFactory<Program> factory) 
        {
            _factory = factory;
            _mapper = new Mapper(new MapperConfiguration(a => 
            {
                a.CreateMap<ApplicationUser, EmployeeRegisterViewModel>().IncludeMembers(a => a.Address, a => a.BusinessEmployee).ReverseMap();
                a.CreateMap<Address, EmployeeRegisterViewModel>().ForMember(a => a.Id, b => b.Ignore()).ReverseMap().ForMember(a => a.Id, b => b.Ignore());
                a.CreateMap<BusinessEmployee, EmployeeRegisterViewModel>().ForMember(a => a.Id, b => b.Ignore()).ReverseMap().ForMember(a => a.Id, b => b.Ignore());
            }));
            _imageUtility = _factory.Services.GetRequiredService<ImageUtility>();
        }
    }
}
