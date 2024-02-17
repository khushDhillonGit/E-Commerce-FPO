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
        public bool IsEmployeeById(Guid id)
        {
            if (id == Guid.Empty) return false;
            var userRoles = _context.UserRoles.Include(a => a.Role).Where(a => a.UserId == id).Select(a=>a.Role.Name);
            return userRoles.Contains(ApplicationRole.Employee);
        }
        protected bool IsAuthorisedForBusiness(Guid userId, Guid bId)
        {
            var data = _context.Users.Include(a=>a.Businesses).Include(a=>a.BusinessEmployee).Select(a=>new { UserId = a.Id,BusinessIds = a.Businesses.Select(a => a.Id), EmpBusinessId = (a.BusinessEmployee == null ?  Guid.Empty : a.BusinessEmployee.BusinessId) }).FirstOrDefault(a=>a.UserId == userId);

            if(data == null) return false;
            if (data.EmpBusinessId == bId || data.BusinessIds.Contains(bId)) return true;
            return false;
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
                CurrentBusinessName = _context.Businesses.FirstOrDefault(a=>a.Id == value)?.Name ?? "";
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
