using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        public override void OnActionExecuted(ActionExecutedContext filter) 
        {
            ViewData[Constants.CurrentBusinessName] = CurrentBusinessName;
            base.OnActionExecuted(filter);
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

        protected bool IsBusinessOwner(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return Task.Run(async () => await _userManager.IsInRoleAsync(user, ApplicationRole.BusinessOwner)).Result;
        }

        protected bool IsCustomer(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return Task.Run(async () => await _userManager.IsInRoleAsync(user, ApplicationRole.Customer)).Result;
        }
        protected bool IsEmployee(ApplicationUser user)
        {
            if (user == null || user.Id == Guid.Empty) return false;
            return Task.Run(async () => await _userManager.IsInRoleAsync(user, ApplicationRole.Employee)).Result;
        }
        protected bool IsAuthorisedForBusiness(ApplicationUser user, Guid bId)
        {
            //if user in employee check if it belongs to this business
            if (IsEmployee(user) && user.BusinessEmployee?.BusinessId != bId) return false;
            //this case would be business owner have this business
            if (user.Businesses.FirstOrDefault(a => a.Id == bId) == null) return false;
            return true;

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
                HttpContext.Session.SetString(Constants.CurrentBusinessId, value.ToString());
            }
        }

        public string CurrentBusinessName {
            get
            {
                string? name = HttpContext.Session.GetString(Constants.CurrentBusinessName);
                if (name == null) return string.Empty;
                return name;
            }
            set
            {
                HttpContext.Session.SetString(Constants.CurrentBusinessName, value.ToString());
            }
        }
    }
}
