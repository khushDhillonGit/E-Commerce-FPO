using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerce.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger,UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            if (await IsBusinessOwner(user)) 
            {
                return RedirectToAction("Index","Businesses");
            }

            if (await IsEmployee(user)) 
            {
                var bId = _context.BusinessEmployees.FirstOrDefault(a=>a.Id == user.Id)?.BusinessId;
                if(bId == null) return NotFound();
                return RedirectToAction("CurrentBusiness", "Businesses", new { businessId = bId });

            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error(int code,string? message) 
        {
            ViewData["Message"] = "One of our function is dead, Sorry for inconviniance";
            _logger.LogError("Error: " + message + "\nDate: " + DateTimeOffset.Now.ToString());
            if (message != null)
            {
                ViewData["Message"] = message;
            }
            return View();
        }
    }
}