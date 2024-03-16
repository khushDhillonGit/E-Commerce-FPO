using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Data.Models;
using ECommerce.Data;

namespace ECommerce.Controllers
{
    [Authorize(Roles = $"{ApplicationRole.SuperAdmin}")]
    public class BusinessCategoriesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BusinessCategoriesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
        }

        // GET: BusinessCategories
        public async Task<IActionResult> Index()
        {
              return _context.BusinessCategories != null ? 
                          View(await _context.BusinessCategories.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BusinessCategories'  is null.");
        }

        // GET: BusinessCategories/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.BusinessCategories == null)
            {
                return NotFound();
            }

            var businessCategory = await _context.BusinessCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessCategory == null)
            {
                return NotFound();
            }

            return View(businessCategory);
        }

        // GET: BusinessCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BusinessCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,IsDelete,CreatedDate")] BusinessCategory businessCategory)
        {
            if (ModelState.IsValid)
            {
                businessCategory.Id = Guid.NewGuid();
                businessCategory.CreatedDate = DateTimeOffset.UtcNow;
                _context.Add(businessCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(businessCategory);
        }

        // GET: BusinessCategories/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BusinessCategories == null)
            {
                return NotFound();
            }

            var businessCategory = await _context.BusinessCategories.FindAsync(id);
            if (businessCategory == null)
            {
                return NotFound();
            }
            return View(businessCategory);
        }

        // POST: BusinessCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Description")] BusinessCategory businessCategory)
        {
            if (id != businessCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessCategoryExists(businessCategory.Id))
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
            return View(businessCategory);
        }

        // GET: BusinessCategories/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.BusinessCategories == null)
            {
                return NotFound();
            }

            var businessCategory = await _context.BusinessCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessCategory == null)
            {
                return NotFound();
            }

            return View(businessCategory);
        }

        // POST: BusinessCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.BusinessCategories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BusinessCategories'  is null.");
            }
            var businessCategory = await _context.BusinessCategories.FindAsync(id);
            if (businessCategory != null)
            {
                _context.BusinessCategories.Remove(businessCategory);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessCategoryExists(Guid id)
        {
          return (_context.BusinessCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
