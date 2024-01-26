using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AutoMapper;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.BusinessOwner},{ApplicationRole.SuperAdmin}")]
    public class BusinessesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUtility _imageUtility;
        private readonly Mapper _mapper;

        public BusinessesController(ImageUtility imageUtility, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
            _imageUtility = imageUtility;
            var mapperConfig = new MapperConfiguration(e=> 
            {
                e.CreateMap<AddressViewModel,Address>();
                e.CreateMap<BusinessViewModel,Business>();
            });
            _mapper = new Mapper(mapperConfig);
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();
            return View(user.Businesses);
        }

        private SelectList GetBusinessCategoriesSelectList() 
        {
            var businessCategories = _context.BusinessCategories.Select(a => new { Id = a.Id, Name = a.Name }).ToList();
            return new SelectList(businessCategories, "Id", "Name", "General");
        }

        public class BusinessModel
        {
            public BusinessViewModel Business { get; set; } = new BusinessViewModel();
            public AddressViewModel Address { get; set; } = new AddressViewModel();
            public SelectList? Categories { get; set; }
            public IFormFile? Image { get; set; }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BusinessModel();
            viewModel.Categories = GetBusinessCategoriesSelectList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BusinessModel businessModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await GetCurrentUserAsync();
                    if (user == null) return Unauthorized();

                    Address address = _mapper.Map<Address>(businessModel.Address);

                    Business business = _mapper.Map<Business>(businessModel.Business);
                    business.Address = address;    

                    if (businessModel.Image != null)
                    {
                        try
                        {
                            business.ImageUrl = await _imageUtility.SaveImageToServerAsync(businessModel.Image, Path.Combine("images", "businesses"));
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                        }
                    }


                    business.Owners.Add(user);

                    _context.Businesses.Add(business);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) 
                {
                    Log.Logger.Error(ex, "{Date}, Message:{Message}", DateTimeOffset.UtcNow, ex.Message);
                    ModelState.AddModelError("ErrorMessage", "Something went wrong, Please contact administrator");
                }
                return RedirectToAction(nameof(Create));
            }
            businessModel.Categories = GetBusinessCategoriesSelectList();
            return View(businessModel);
        }

        protected override async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userName = this.HttpContext?.User?.Identity?.Name;
            if (userName != null)
            {
                return await _context.Users.Include(a => a.Businesses).FirstOrDefaultAsync(a => a.UserName == userName);
            }
            return null;
        }


    }
}
