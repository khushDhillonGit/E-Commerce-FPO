using AutoMapper;
using JattanaNursury.Data;
using JattanaNursury.Models;
using JattanaNursury.Models.Api;
using JattanaNursury.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JattanaNursury.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.Admin},{ApplicationRole.Employee}")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult PaidOrders() 
        {
            var orders = _context.Orders.AsEnumerable().Where(a=>a.FullyPaid);
            List<OrderPaidModel> result = new();
            foreach (var order in orders)
            {
                var model = new OrderPaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, CustomerName = order.Customer?.CustomerName, CustomerPhone = order.Customer?.PhoneNumber, CustomerAddress = order.Customer?.FullAddress };

                foreach (var product in order.ProductOrders)
                {
                    model.Products.Add(new OrderProductsModel { ProductId = product.ProductId, Name = product.Product?.Name, Quantity = product.Quantity, TotalPrice = product.TotalPrice, SellingPrice = product.Product?.SellingPrice, UnitPrice = product.Product?.UnitPrice });
                }

                result.Add(model);
            }
            return View(result);
        }

        public IActionResult UnpaidOrders()
        {
            var orders = _context.Orders.Where(a => !a.FullyPaid);
            List<OrderUnpaidModel> result = new();
            foreach (var order in orders)
            {
                var model = new OrderUnpaidModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount, Employee = order.EmployeeId, PaidByCustomer = order.PaidByCustomer, CustomerName = order.Customer?.CustomerName, CustomerPhone = order.Customer?.PhoneNumber, CustomerAddress = order.Customer?.FullAddress };

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

        public class ProductModel
        {
            public Guid Id { get; set; }
            public string? Name { get; set; }
            public decimal SellingPrice { get; set; }
            public decimal Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        [HttpGet]
        public async Task<JsonResult> GetProductsByNameAsync(string search = "")
        {
            List<ProductModel> products = new();
            try
            {
                var pList = await _context.Products.Where(a => a.Name.ToLower().Contains(search.ToLower())).ToListAsync();
                var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Product, ProductModel>()));
                products = mapper.Map<List<Product>, List<ProductModel>>(pList);
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
                if (ModelState.IsValid)
                {
                    var customerId = AddCustomer(saleOrder);

                    var order = new Order { CustomerId = customerId, OrderDate = DateTimeOffset.UtcNow, Discount = saleOrder.Discount, EmployeeId = saleOrder.Employee, PaidByCustomer = saleOrder.PaidByCustomer };

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

        private Guid AddCustomer(OrderViewModel saleOrder) 
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<OrderViewModel, Customer>()));
            var customer = mapper.Map<OrderViewModel, Customer>(saleOrder);
            customer.CreatedDate = DateTimeOffset.UtcNow;
            _context.Customers.Add(customer);
            return customer.Id;
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


    }
}
