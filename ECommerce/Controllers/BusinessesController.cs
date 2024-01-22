using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.BusinessOwner},{ApplicationRole.SuperAdmin}")]
    public class BusinessesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUtility _imageUtility;
        public BusinessesController(ImageUtility imageUtility, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
            _imageUtility = imageUtility;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();
            return View(user.Businesses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var businessCategories = _context.BusinessCategories.Select(a => new { Id = a.Id, Name = a.Name }).ToList();
            ViewData["Categories"] = new SelectList(businessCategories, "Id", "Name", "General");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBusinessViewModel businessModel)
        {
            if (ModelState.IsValid)
            {

                var user = await GetCurrentUserAsync();
                if (user == null) return Unauthorized();

                var Address = new Address() { StreetAddress = businessModel.StreetAddress, UnitApt = businessModel.UnitApt, City = businessModel.City, Country = businessModel.Country, PostalCode = businessModel.PostalCode, Province = businessModel.Province, CreatedDate = DateTimeOffset.UtcNow };

                Business business = new Business() { Address = Address, BusinessCategoryId = businessModel.BusinessCategoryId, CreatedDate = DateTimeOffset.UtcNow, Name = businessModel.Name, Description = businessModel.Description, Phone = businessModel.Phone };

                if (businessModel.Image != null)
                {
                    try 
                    {
                        business.ImageUrl = await _imageUtility.SaveImageToServerAsync(businessModel.Image, Path.Combine("images", "businesses"));
                    }
                    catch(Exception ex) 
                    {
                        Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                    }
                }


                business.Owners.Add(user);


            }
            return View();
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
