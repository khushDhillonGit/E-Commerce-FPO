using ECommerce.Data;
using ECommerce.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.ViewModels;
using AutoMapper;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Serilog;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net;
using ECommerce.Services;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
    public class EmployeesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Mapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ImageUtility _imageUtility;
        public EmployeesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ImageUtility imageUtility) : base(userManager, context)
        {
            _context = context;
            _userManager = userManager;
            _mapper = new Mapper(new MapperConfiguration(a =>
            {
                a.CreateMap<ApplicationUser, EmployeeRegisterViewModel>().IncludeMembers(a => a.Address).ReverseMap();
                a.CreateMap<Address, EmployeeRegisterViewModel>().ReverseMap();

            }));
            _emailSender = emailSender;
            _signInManager = signInManager;
            _configuration = configuration;
            _imageUtility = imageUtility;
        }

        public async Task<IActionResult> Index()
        {
            var business = await GetCurrentUserBusinessAsync();
            if (business == null) return NotFound();

            List<EmployeeRegisterViewModel> viewModel = new List<EmployeeRegisterViewModel>();

            try
            {
                foreach (var employee in business.Employees.Select(a => a.Employee))
                {
                    var employeeRegister = _mapper.Map<EmployeeRegisterViewModel>(employee);
                    employeeRegister.BusinessesName = CurrentBusinessName;
                    viewModel.Add(employeeRegister);
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                return BadRequest();
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterEmployeeAsync()
        {
            EmployeeRegisterViewModel viewModel = new EmployeeRegisterViewModel();
            viewModel.BusinessesList = await GetCurrentUserBusinessesSelectListAsync();
            return View(viewModel);
        }

        private async Task<SelectList?> GetCurrentUserBusinessesSelectListAsync()
        {
            var user = await GetCurrentUserAsync();
            return user == null ? throw new UnauthorizedAccessException() : new SelectList(user.Businesses, "Id", "Name", CurrentBusinessId);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPassword), new { userId });
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();
            return View(new ResetPasswordViewModel() { UserId = userId });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel viewModel)
        {
            string message = "Error resetting password please contact IT";
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
                if (user == null) return BadRequest();
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, viewModel.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                message = result.Errors.Single().Description;
            }
            ModelState.AddModelError("ErrorMessage", message);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployeeAsync(EmployeeRegisterViewModel registerViewModel, IFormFile? image)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = _mapper.Map<EmployeeRegisterViewModel, ApplicationUser>(registerViewModel);
                    await _userManager.SetUserNameAsync(user, registerViewModel.Email);
                    await _userManager.SetEmailAsync(user, registerViewModel.Email);

                    if (image != null)
                    {
                        user.ImageUrl = await _imageUtility.SaveImageToServerAsync(image, "users");
                    }
                    else 
                    {
                        user.ImageUrl = Path.Combine("images", "default", "default-user-image.png");
                    }
                    BusinessEmployee businessEmployee = new BusinessEmployee() { EmployeeId = user.Id, BusinessId = registerViewModel.BusinessId };
                    user.BusinessEmployee = businessEmployee;

                    string? password = _configuration["DefaultEmployeePassword"];
                    password ??= "Password!123";
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, ApplicationRole.Employee);


                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callBackUrl = Url.Action("ConfirmEmail", "Employees", new { userId = user.Id, code = code }, Request.Scheme);
                        await _emailSender.SendEmailAsync(user.Email, $"Welcome to {CurrentBusinessName} employee program",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callBackUrl)}'>clicking here</a>.");

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                ModelState.AddModelError("ErrorMessage", "Something went wrong, Please contact IT team");
            }

            registerViewModel.BusinessesList = await GetCurrentUserBusinessesSelectListAsync();
            return View(registerViewModel);
        }

        private async Task<Business?> GetCurrentUserBusinessAsync()
        {
            if (CurrentBusinessId == Guid.Empty) return null;

            return await _context.Businesses.Include(a => a.Employees).ThenInclude(a => a.Employee).ThenInclude(a => a.Address).Include(a => a.Address).FirstOrDefaultAsync(a => a.Id == CurrentBusinessId);
        }
        protected override async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var userName = this.HttpContext?.User?.Identity?.Name;
            if (userName != null)
            {
                return await _context.Users.Include(a => a.Address).Include(a => a.Businesses).ThenInclude(a => a.Employees).ThenInclude(a => a.Employee).ThenInclude(a => a.Address).Include(a => a.BusinessEmployee).Include(a => a.Businesses).ThenInclude(a => a.Address).FirstOrDefaultAsync(a => a.UserName == userName);
            }
            return null;
        }

    }
}
