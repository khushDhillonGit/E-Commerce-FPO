using ECommerce.Data.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data
{
    public class ContextSeed
    {

        public readonly ApplicationDbContext _context;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<ApplicationRole> _roleManager;

        public ContextSeed(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public static List<ApplicationRole> applicationRoles = new()
        {
            new ApplicationRole(){Name = ApplicationRole.SuperAdmin,NormalizedName = ApplicationRole.SuperAdmin.ToUpper() },
            new ApplicationRole(){Name = ApplicationRole.BusinessOwner,NormalizedName = ApplicationRole.BusinessOwner.ToUpper() },
            new ApplicationRole(){Name = ApplicationRole.Customer,NormalizedName = ApplicationRole.Customer.ToUpper() },
            new ApplicationRole(){Name = ApplicationRole.Employee,NormalizedName = ApplicationRole.Employee.ToUpper() },
        };
        public async Task SeedUsersAndRoles()
        {
            var roleNames = _roleManager.Roles.Select(a => a.Name).ToList();
            foreach (var item in applicationRoles)
            {
                if (!roleNames.Contains(item.Name)) await _roleManager.CreateAsync(item);
            }
            var roles = _context.Roles;
            foreach (var item in applicationUsers)
            {
                if (await _userManager.FindByIdAsync(item.Id.ToString()) == null)
                {
                    await _userManager.SetUserNameAsync(item, item.UserName);
                    await _userManager.SetEmailAsync(item, item.Email);
                    await _userManager.ConfirmEmailAsync(item, await _userManager.GenerateEmailConfirmationTokenAsync(item));
                    if (item.Name == "Khush SuperAdmin")
                    {
                        var res = await _userManager.CreateAsync(item, "Hidden");
                    }
                    else
                    {
                        await _userManager.CreateAsync(item, "Password!1");
                    }
                    await _userManager.AddToRoleAsync(item, item.Name.Substring(item.Name.IndexOf(' ') + 1));
                }
            }
            var users = _context.Users;
        }


        public static List<ApplicationUser> applicationUsers = new()
        {
            new ApplicationUser
            {
                Id = Guid.Parse("B56220D0-3A02-4045-97B7-DACB8D17AAE6"),
                Email = "khushwinder.dhillon@outlook.com",
                EmailConfirmed = true,
                Name = "Khush " + ApplicationRole.SuperAdmin,
                NormalizedEmail = "khushwinder.dhillon@outlook.com".ToUpper(),
                UserName = "khushwinder.dhillon@outlook.com",
                NormalizedUserName = "khushwinder.dhillon@outlook.com".ToUpper(),
                PhoneNumber = "+9999999999",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            },

            new ApplicationUser
            {
                Id = Guid.Parse("E69A7462-C405-41A2-952A-1BF7103D191A"),
                Email = "testBO@outlook.com",
                EmailConfirmed = true,
                Name = "Test " + ApplicationRole.BusinessOwner,
                NormalizedEmail = "testBO@outlook.com".ToUpper(),
                UserName = "testBO@outlook.com",
                NormalizedUserName =  "testBO@outlook.com".ToUpper(),
                PhoneNumber = "+9999999999",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
            },

            new ApplicationUser
            {
                Id = Guid.Parse("1092AB72-02BF-4584-AC5D-0BC2B2AB9792"),
                Email = "testC@outlook.com",
                EmailConfirmed = true,
                Name = "Test " + ApplicationRole.Customer,
                NormalizedEmail = "testC@outlook.com".ToUpper(),
                UserName = "testC@outlook.com",
                NormalizedUserName = "testC@outlook.com".ToUpper(),
                PhoneNumber = "+9999999999",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            },

            new ApplicationUser
            {
                Id = Guid.Parse("747D0D53-3FF9-42C1-BBA6-636F87949451"),
                Email = "testE@outlook.com",
                EmailConfirmed = true,
                Name = "Test " + ApplicationRole.Employee,
                NormalizedEmail = "testE@outlook.com".ToUpper(),
                UserName = "testE@outlook.com",
                NormalizedUserName = "testE@outlook.com".ToUpper(),
                PhoneNumber = "+9999999999",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false
            },

        };
    }
}
