using AutoMapper;
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Models.Api;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner},{ApplicationRole.Employee}")]
    public class OrdersController : BaseController 
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> AllPaidOrdersAsync() 
        {
            var user = await GetCurrentUserWithOrdersAsync();
            if(user == null) return Unauthorized();
            List<OrderPaidModel> result = new();
            foreach (var order in user.Businesses.SelectMany(a=>a.Orders))
            {
                var model = new OrderPaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.LocalDateTime.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, CustomerName = order.CustomerName, CustomerPhone = order.CustomerPhone };

                foreach (var product in order.ProductOrders)
                {
                    model.Products.Add(new OrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product?.SellingPrice, UnitPrice = product.Product?.UnitPrice });
                }

                result.Add(model);
            }
            return View(nameof(PaidOrders),result);
        }

        public async Task<IActionResult> AllUnpaidOrdersAsync()
        {
            var user = await GetCurrentUserWithOrdersAsync();
            if (user == null) return Unauthorized();
            List<OrderUnpaidModel> result = new();
            foreach (var order in user.Businesses.SelectMany(a=>a.Orders))
            {
                var model = new OrderUnpaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.LocalDateTime.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, PaidByCustomer = order.PaidByCustomer, CustomerName = order.CustomerName, CustomerPhone = order.CustomerPhone };

                foreach (var product in order.ProductOrders)
                {
                    model.Products.Add(new OrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product?.SellingPrice, UnitPrice = product.Product?.UnitPrice });
                }

                result.Add(model);
            }
            return View(nameof(UnpaidOrders), result);
        }

        public async Task<IActionResult> PaidOrders() 
        {
            var user = await GetCurrentUserAsync();
            if (user == null || !(IsAuthorisedForBusiness(user,CurrentBusinessId))) { return RedirectToAction("Index", "Home"); }

            var orders = _context.Orders.Include(a=>a.ProductOrders).Where(a=>a.FullyPaid && a.BusinessId == CurrentBusinessId);
            List<OrderPaidModel> result = new();
            foreach (var order in orders)
            {
                var model = new OrderPaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.LocalDateTime.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, CustomerName = order.CustomerName, CustomerPhone = order.CustomerPhone };

                foreach (var product in order.ProductOrders)
                {
                    model.Products.Add(new OrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product?.SellingPrice, UnitPrice = product.Product?.UnitPrice });
                }

                result.Add(model);
            }
            return View(result);
        }

        public async Task<IActionResult> UnpaidOrders()
        {
            var user = await GetCurrentUserAsync();
            if (user == null || !(IsAuthorisedForBusiness(user, CurrentBusinessId))) { return RedirectToAction("Index", "Home"); }

            var orders = _context.Orders.Include(a=>a.ProductOrders).Where(a => a.BusinessId == CurrentBusinessId && !a.FullyPaid);
            List<OrderUnpaidModel> result = new();
            foreach (var order in orders)
            {
                var model = new OrderUnpaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.LocalDateTime.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, PaidByCustomer = order.PaidByCustomer, CustomerName = order.CustomerName, CustomerPhone = order.CustomerPhone};

                foreach (var product in order.ProductOrders) 
                {
                    model.Products.Add(new OrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product?.SellingPrice, UnitPrice = product.Product?.UnitPrice });
                }

                result.Add(model);
            }
            return View(result);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetProductsByNameAsync(string search = "")
        {
            List<ProductViewModel> products = new();
            try
            {
                var pList = await _context.Products.Include(a=>a.Category).Where(a => a.Category.BusinessId == CurrentBusinessId && a.Name.ToLower().Contains(search.ToLower())).ToListAsync();
                var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductViewModel>()));
                products = mapper.Map<List<Product>, List<ProductViewModel>>(pList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Json(products);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSaleOrderAsync([FromBody] OrderViewModel saleOrder)
        {
            try
            {
                var user = await GetCurrentUserAsync(); 
                if(user == null || !IsAuthorisedForBusiness(user,CurrentBusinessId)) return RedirectToAction("Index","Home");

                if (ModelState.IsValid)
                {
                   
                    var order = new Order { OrderDate = DateTimeOffset.UtcNow, Discount = saleOrder.Discount, EmployeeId = saleOrder.Employee, PaidByCustomer = saleOrder.PaidByCustomer ,CustomerName = saleOrder.CustomerName, CustomerPhone = saleOrder.PhoneNumber,BusinessId = CurrentBusinessId};

                    try
                    {
                        order.Price = await AddProductOrdersReturnTotalPriceAsync(order, saleOrder);
                        SetDiscountAndPrice(order, saleOrder);
                        if (order.BillPrice < saleOrder.PaidByCustomer) 
                        {
                            throw new Exception("Customer paid more than Bill price");
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(error: ex.Message);
                    }


                    if (order.BillPrice == saleOrder.PaidByCustomer)
                    {
                        order.FullyPaid = true;
                    }

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    PostBackModel postBack = new PostBackModel(){ Success = true, RedirectUrl = $"/{typeof(OrdersController).Name.Replace("Controller", "")}/{nameof(UnpaidOrders)}", ResponseText = "Order Saved Successfully" };

                    if (order.FullyPaid) 
                    {
                        postBack.RedirectUrl = $"/{typeof(OrdersController).Name.Replace("Controller", "")}/{nameof(PaidOrders)}";
                    }

                    return Json(postBack);
                }
                return BadRequest(error: ModelState.Values.SelectMany(a=>a.Errors)?.FirstOrDefault()?.ErrorMessage);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        private async Task<decimal> AddProductOrdersReturnTotalPriceAsync(Order order,OrderViewModel saleOrder) 
        {
            decimal result = 0;
            foreach (var item in saleOrder.Products)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null || item.Quantity < 1 || product.Quantity < item.Quantity)
                {
                    throw new InvalidDataException("Quantity is invalid");
                }
                var productOrder = new ProductOrder { ProductId = product.Id, Quantity = item.Quantity, TotalPrice = product.SellingPrice * item.Quantity };
                result += productOrder.TotalPrice;
                order.ProductOrders?.Add(productOrder);
                product.Quantity -= item.Quantity;
            }
            return result;
        }

        private void SetDiscountAndPrice(Order order, OrderViewModel saleOrder) 
        {
            var discountPercetage = (saleOrder.Discount / order.Price) * 100;

            if (discountPercetage > 20)
            {
                throw new Exception("Invalid Discount");
            }

            order.BillPrice = order.Price - saleOrder.Discount;
            var totalOrders = _context.Orders.Count();
            order.OrderNumber = (totalOrders + 1001).ToString();
        }

        private async Task<ApplicationUser?> GetCurrentUserWithOrdersAsync()
        {
            var userName = this.HttpContext?.User?.Identity?.Name;
            if (userName != null)
            {
                return await _context.Users.Include(a => a.Businesses).ThenInclude(a => a.Orders).ThenInclude(a=>a.ProductOrders).Include(a=>a.Businesses).ThenInclude(a=>a.Address).FirstOrDefaultAsync(a => a.UserName == userName);
            }
            return null;
        }

    }
}
