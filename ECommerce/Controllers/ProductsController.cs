using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ECommerce.Services;
using Serilog;

namespace ECommerce.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageUtility _imageUtility;

        public ProductsController(ImageUtility imageUtility,UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
            _imageUtility = imageUtility;
        }



        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
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
                        Log.Logger.Error(ex,"{Date}: {Message}",DateTimeOffset.UtcNow, ex.Message);
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

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
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
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(Guid id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
