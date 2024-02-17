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
using ECommerce.Models.Api;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.BusinessOwner},{ApplicationRole.SuperAdmin},{ApplicationRole.Employee}")]
    public class BusinessesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUtility _imageUtility;
        private readonly Mapper _mapper;
        private readonly string imageSavePath = Path.Combine("images", "businesses");

        public BusinessesController(ImageUtility imageUtility, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
            _imageUtility = imageUtility;
            var mapperConfig = new MapperConfiguration(e =>
            {
                e.CreateMap<AddressViewModel, Address>().ReverseMap();
                e.CreateMap<BusinessViewModel, Business>().ReverseMap();
            });
            _mapper = new Mapper(mapperConfig);
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if(user == null) return NotFound();
            List<BusinessViewModel> businesses = new List<BusinessViewModel>();
            foreach (var business in user.Businesses)
            {
                BusinessViewModel vm = _mapper.Map<BusinessViewModel>(business);

                var bussinessData = _context.Businesses.Include(a => a.ProductCategories).ThenInclude(a => a.Products).Include(a => a.Orders).Include(a => a.Employees).Select(a => new
                {
                    a.Id,
                    TotalProducts = a.ProductCategories.SelectMany(a => a.Products).Count(),
                    TotalOrders = a.Orders.Count(),
                    TotalCategories = a.ProductCategories.Count(),
                    TotalEmployees = a.Employees.Count(),
                }).FirstOrDefault(a => a.Id == business.Id);

                if (bussinessData != null)
                {

                    vm.TotalProducts = bussinessData.TotalProducts;
                    vm.TotalOrders = bussinessData.TotalOrders;
                    vm.TotalProducts = bussinessData.TotalCategories;
                    vm.TotalProducts = bussinessData.TotalEmployees;

                }
                businesses.Add(vm);
            }

            return View(businesses);
        }

        public async Task<IActionResult> CurrentBusiness(Guid bId)
        {
            var user = await GetCurrentUserAsync();
            if(bId == Guid.Empty) bId = CurrentBusinessId;
            if (user == null || !IsAuthorisedForBusiness(user.Id, bId)) return Unauthorized();
            // store current bId
            CurrentBusinessId = bId;

            var business = _context.Businesses.Include(a => a.ProductCategories).ThenInclude(a => a.Products).Include(a => a.Orders).Include(a => a.Employees).FirstOrDefault(a => a.Id == CurrentBusinessId);
            if(business == null) return NotFound();

            BusinessViewModel vm = _mapper.Map<BusinessViewModel>(business);

            vm.TotalProducts = business.ProductCategories.SelectMany(a => a.Products).Count();
            vm.TotalOrders = business.Orders.Count;
            vm.TotalCategories = business.ProductCategories.Count;
            vm.TotalEmployees = business.Employees.Count;

            return View(vm);
        }


        private SelectList GetBusinessCategoriesSelectList()
        {
            var businessCategories = _context.BusinessCategories.Select(a => new { a.Id, a.Name }).ToList();
            return new SelectList(businessCategories, "Id", "Name", "General");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BusinessViewModel();
            viewModel.Categories = GetBusinessCategoriesSelectList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BusinessViewModel businessModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await GetCurrentUserAsync();
                    if (user == null) return Unauthorized();

                    Business business = _mapper.Map<Business>(businessModel);
                    try
                    {
                        if (businessModel.Image != null)
                        {
                            business.ImageUrl = await _imageUtility.SaveImageToServerAsync(businessModel.Image, imageSavePath);
                        }
                        else
                        {
                            business.ImageUrl = Path.Combine("images", "default", "default-business.png");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
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



        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var business = await _context.Businesses.Include(a => a.Address).FirstOrDefaultAsync(x => x.Id == id);
            BusinessViewModel viewModel = _mapper.Map<BusinessViewModel>(business);
            viewModel.Categories = GetBusinessCategoriesSelectList();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, BusinessViewModel businessModel)
        {
            if (id != businessModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var business = await _context.Businesses.Include(a => a.Address).FirstOrDefaultAsync(a => a.Id == businessModel.Id);
                    if (business == null) return NotFound();
                    _mapper.Map<BusinessViewModel, Business>(businessModel, business);
                    if (businessModel.Image != null)
                    {
                        try
                        {
                            business.ImageUrl = await _imageUtility.SaveImageToServerAsync(businessModel.Image, imageSavePath);
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                        }
                    }
                    _context.Businesses.Update(business);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "{Date}, Message:{Message}", DateTimeOffset.UtcNow, ex.Message);
                    ModelState.AddModelError("ErrorMessage", "Something went wrong, Please contact administrator");
                }
            }

            businessModel.Categories = GetBusinessCategoriesSelectList();
            return View(businessModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                Business? business = await _context.Businesses.Include(a => a.ProductCategories).ThenInclude(a => a.Products).Include(a => a.Employees).Include(a => a.Address).Include(a => a.Orders).FirstOrDefaultAsync(a => a.Id == id);

                if (business == null)
                {
                    return NotFound();
                }

                _context.Orders.RemoveRange(business.Orders);
                _context.Products.RemoveRange(business.ProductCategories.SelectMany(a => a.Products));
                _context.Categories.RemoveRange(business.ProductCategories);
                _context.BusinessEmployees.RemoveRange(business.Employees);
                _context.Addresses.Remove(business.Address ?? new Address());
                _context.Businesses.Remove(business);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                return BadRequest();
            }

            return Ok(new PostBackModel { Success = true, RedirectUrl = "/Businesses/index" });
        }

    

        protected override async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userName = this.HttpContext?.User?.Identity?.Name;
            if (userName != null)
            {
                return await _context.Users.Include(a => a.Businesses).ThenInclude(a => a.Address).Include(a => a.BusinessEmployee).FirstOrDefaultAsync(a => a.UserName == userName);
            }
            return null;
        }
    }
}
