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
        public EmployeesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IConfiguration configuration) : base(userManager, context)
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
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterEmployee()
        {
            return View(new EmployeeRegisterViewModel());
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if(userId == null || code == null) return BadRequest();
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return Unauthorized();
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded) 
            {
                return RedirectToAction(nameof(ResetPasswordAsync), new { userId });
            }
            return BadRequest();
        }

        public class ResetPasswordViewModel
        {
            public Guid UserId { get; set; }
            [Required]
            [Compare(nameof(ConfirmPassword))]
            public string NewPassword { get; set; }
            [Required]
            public string ConfirmPassword { get; set; }
            [Required]
            public string Code { get; set; } 
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(Guid userId,string code)
        {
            return View(new ResetPasswordViewModel() { UserId = userId,Code = code});
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid) 
            {
                var user = await _userManager.FindByIdAsync(viewModel.UserId.ToString());
                if(user == null) return BadRequest();
                viewModel.Code =  Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(viewModel.Code));
                var result = await _userManager.ResetPasswordAsync(user, viewModel.Code ,viewModel.NewPassword);
                if (result.Succeeded) 
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("ErrorMessage","Error resetting password please contact IT");
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterEmployeeAsync(EmployeeRegisterViewModel registerViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = _mapper.Map<EmployeeRegisterViewModel,ApplicationUser>(registerViewModel);
                    await _userManager.SetUserNameAsync(user, registerViewModel.Email);
                    await _userManager.SetEmailAsync(user, registerViewModel.Email);

                    string? password = _configuration["DefaultEmployeePassword"];
                    password ??= "Password!123";
                    var result = await _userManager.CreateAsync(user,password);

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
            return View(registerViewModel);
        }


    }
}
