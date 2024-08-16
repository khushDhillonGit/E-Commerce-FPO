using ECommerce.Data;
using ECommerce.Data.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Authorize]
    public class CustomerOrderController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public CustomerOrderController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllOrdersAsync()
        {
            var user = await GetCurrentUserWithOrdersAsync();
            if (user == null) { return NotFound(); }
            List<CustomerOrderViewModel> result = new();
            foreach (var order in user.CustomerOrders)
            {
                var model = new CustomerOrderViewModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.LocalDateTime.ToString("o"), BillPrice = order.FinalBill, Price = order.TotalPrice, Discount = order.Discount };

                foreach (var product in order.CustomerOrderProducts)
                {
                    model.Products.Add(new CustomerOrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product.SellingPrice });
                }

                result.Add(model);
            }
            return View("AllOrders", result);
        }

        private async Task<ApplicationUser?> GetCurrentUserWithOrdersAsync()
        {
            var userName = this.HttpContext?.User?.Identity?.Name;
            if (userName != null)
            {
                return await _context.Users.Include(a => a.CustomerOrders).ThenInclude(a => a.CustomerOrderProducts).ThenInclude(a => a.Product).FirstOrDefaultAsync(a => a.UserName == userName);
            }
            return null;
        }
    }
}
