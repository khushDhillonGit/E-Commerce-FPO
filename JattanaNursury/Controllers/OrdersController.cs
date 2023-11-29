using AutoMapper;
using JattanaNursury.Data;
using JattanaNursury.Models;
using JattanaNursury.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace JattanaNursury.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() 
        {
            var orders = _context.Orders;
            List<OrderIndexViewModel> result = new();
            foreach (var order in orders)
            {
                result.Add(new OrderIndexViewModel { OrderNumber = order.OrderNumber, OrderId = order.Id, OrderDate = order.OrderDate.ToString("o"), BillPrice = order.BillPrice, Price = order.Price, Discount = order.Discount,Employee = order.EmployeeId });
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
        public async Task<List<ProductModel>> GetProductsByNameAsync(string search = "")
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
            return products;
        }

        [HttpPost]
        public async Task<IActionResult> SaveSaleOrderAsync([FromBody] OrderViewModel saleOrder)
        {

            if (ModelState.IsValid)
            {
                if (saleOrder == null) return View(nameof(Create));

                if (!saleOrder.IsPaid && (string.IsNullOrEmpty(saleOrder.PhoneNumber) || string.IsNullOrEmpty(saleOrder.FullAddress)))
                {
                    return View(nameof(Create));
                }

                var customerId = AddCustomer(saleOrder);

                var order = new Order { CustomerId = customerId, OrderDate = DateTime.UtcNow, Discount = saleOrder.Discount, EmployeeId = saleOrder.Employee, IsPaid = saleOrder.IsPaid };

                try
                {
                    order.Price = await AddProductOrdersReturnTotalPriceAsync(order, saleOrder);
                }
                catch (Exception ex)
                {
                    return View(nameof(Create));
                }

                try
                {
                    SetDiscountAndPrice(order, saleOrder);
                }
                catch (Exception ex)
                {
                    return View(nameof(Create));
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        private Guid AddCustomer(OrderViewModel saleOrder) 
        {
            var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<OrderViewModel, Customer>()));
            var customer = mapper.Map<OrderViewModel, Customer>(saleOrder);
            customer.CreatedDate = DateTime.UtcNow;
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
                var productOrder = new ProductOrder { ProductId = product.Id, Quantity = item.Quantity, TotalPrice = product.UnitPrice * item.Quantity };
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
