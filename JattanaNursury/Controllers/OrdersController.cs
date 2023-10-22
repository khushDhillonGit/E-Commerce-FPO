using AutoMapper;
using JattanaNursury.Data;
using JattanaNursury.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JattanaNursury.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public class ProductModel 
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }

        [HttpGet]
        public async Task<List<ProductModel>> GetProductsByName(string search) 
        {
            List<ProductModel> products = new();
            if (string.IsNullOrEmpty(search)) return products;

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
    }
}
