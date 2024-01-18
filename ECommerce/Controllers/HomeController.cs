using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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