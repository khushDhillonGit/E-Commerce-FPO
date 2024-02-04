using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

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

        protected async Task<bool> IsBusinessOwner(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return await _userManager.IsInRoleAsync(user, ApplicationRole.BusinessOwner);
        }

        protected async Task<bool> IsCustomer(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return await _userManager.IsInRoleAsync(user, ApplicationRole.Customer);
        }
        protected async Task<bool> IsEmployee(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return await _userManager.IsInRoleAsync(user, ApplicationRole.Employee);
        }

        protected Guid CurrentBusinessId
        {
            get
            {
                string? businessId = HttpContext.Session.GetString(Constants.CurrentBusinessId);
                if (businessId == null) return Guid.Empty;
                return Guid.Parse(businessId);
            }
            set
            {
                HttpContext.Session.SetString(Constants.CurrentBusinessId, JsonConvert.SerializeObject(value));
            }
        }
    }
}
