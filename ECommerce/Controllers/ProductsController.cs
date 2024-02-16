using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ECommerce.Services;
using Serilog;
using ECommerce.Models.Api;
using AutoMapper;
using ECommerce.ViewModels;

namespace ECommerce.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUtility _imageUtility;
        private readonly Mapper _mapper;
        public ProductsController(ImageUtility imageUtility, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
            _imageUtility = imageUtility;
            _mapper = new Mapper(new MapperConfiguration(e =>
            {
                e.CreateMap<Product, ProductViewModel>().ReverseMap();
            }));
        }

        public async Task<IActionResult> ProductsOnSale(string? search)
        {
            List<Product> products = new List<Product>();
            if (string.IsNullOrWhiteSpace(search))
            {
                products = await _context.Products.Include(a => a.Category).ThenInclude(a => a.Business).ToListAsync();
            }
            else 
            {
               products = await _context.Products.Include(a => a.Category).ThenInclude(a => a.Business).Where(a =>
               a.Name.Contains(search) ||
               a.Description.Contains(search) ||
               a.Category.Name.Contains(search) ||
               a.Category.Business.Name.Contains(search)).ToListAsync();
               
                ViewBag.Search = search;    
            }
            List<ProductOnSaleViewModel> vm = new List<ProductOnSaleViewModel>();
            foreach (var product in products)
            {
                vm.Add(new ProductOnSaleViewModel() { Id = product.Id, Name = product.Name, Description = product.Description, BusinessName = product.Category?.Business?.Name, CategoryName = product.Category?.Name, ImageUrl = product.ImageUrl });
            }
            return View(vm);
        }
        [Authorize(Roles = $"{ApplicationRole.BusinessOwner},{ApplicationRole.SuperAdmin},{ApplicationRole.Employee}")]
        // GET: Products
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            List<Product> products = new List<Product>();
            if (user != null && IsBusinessOwner(user))
            {
                if (CurrentBusinessId == Guid.Empty) return RedirectToAction("Index", "Businesses");
                products = await _context.Products.Include(a => a.Category).ThenInclude(a => a.Business).Where(a => a.Category.BusinessId == CurrentBusinessId).ToListAsync();
            }

            List<ProductViewModel> viewModel = new List<ProductViewModel>();
            foreach (var product in products)
            {
                ProductViewModel model = _mapper.Map<ProductViewModel>(product);
                model.BusinessName = product.Category?.Business?.Name ?? "";
                model.CategoryName = product.Category?.Name ?? "";
                viewModel.Add(model);
            }
            return View(viewModel);
        }

        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public async Task<IActionResult> AllProducts()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return Unauthorized();
            var products = await _context.Products.Include(a => a.Category).ThenInclude(a => a.Business).ThenInclude(a => a.Owners).Where(a => a.Category.Business.Owners.Select(a => a.Id).Contains(user.Id)).ToListAsync();
            List<ProductViewModel> viewModel = new List<ProductViewModel>();
            foreach (var product in products)
            {
                ProductViewModel model = _mapper.Map<ProductViewModel>(product);
                model.BusinessName = product.Category.Business.Name ?? "";
                viewModel.Add(model);
            }
            ViewData["Layout"] = "~/Views/Shared/_Layout.cshtml";
            return View(nameof(Index), viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(a => a.BusinessId == CurrentBusinessId), "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UnitPrice,SellingPrice,SKU,Quantity,CategoryId")] Product product, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                product.Id = Guid.NewGuid();
                product.CreatedDate = DateTimeOffset.UtcNow;
                if (Image != null)
                {
                    try
                    {
                        product.ImageUrl = await _imageUtility.SaveImageToServerAsync(Image, Path.Combine("images", "product"));
                    }
                    catch (Exception ex)
                    {
                        //TODO:Handle Exception
                        Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                    }
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }



        // GET: Products/Edit/5
        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(a => a.Category).ThenInclude(a => a.Business).FirstOrDefaultAsync(a => a.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            CurrentBusinessId = product.Category.BusinessId;
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(a => a.BusinessId == product.Category.Business.Id), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description,UnitPrice,SellingPrice,SKU,Quantity,CategoryId,ImageUrl")] Product product, IFormFile? Image)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (Image != null)
                    {
                        try
                        {
                            product.ImageUrl = await _imageUtility.SaveImageToServerAsync(Image, Path.Combine("images", "product"));
                        }
                        catch (Exception ex)
                        {
                            //TODO:Handle Exception
                            Log.Logger.Error(ex, "{Date}: {Message}", DateTimeOffset.UtcNow, ex.Message);
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = $"{ApplicationRole.SuperAdmin},{ApplicationRole.BusinessOwner}")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return Ok(new PostBackModel { Success = true, RedirectUrl = "/Products/index" });
        }

        private bool ProductExists(Guid id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
