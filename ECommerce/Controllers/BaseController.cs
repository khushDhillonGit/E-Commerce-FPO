using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public BaseController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) 
        {
            _userManager = userManager;
            _context = context;
        }

        protected virtual async Task<ApplicationUser?> GetCurrentUserAsync() 
        {
            var user = this.HttpContext?.User;
            if (user != null) 
            {
                return await _userManager.GetUserAsync(user);
            }
            return null;
        }
    }
}
