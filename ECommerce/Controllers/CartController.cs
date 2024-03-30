using ECommerce.Data;
using ECommerce.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    public class CartController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public CartController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager, context)
        {
            _context = context;
        }

        private async Task<Cart> CreateCartForUser(Guid userId) 
        {
            var cart = new Cart { CustomerId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return _context.Carts.First(a=>a.Id == cart.Id);
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) { return NotFound(); }
            var cart = await _context.Carts.Include(a=>a.CartProducts).ThenInclude(a=>a.Product).FirstOrDefaultAsync(a=>a.CustomerId == user.Id);
            if (cart == null) cart = await CreateCartForUser(user.Id);
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(Guid productId,int quantity) 
        {
            var product = await _context.Products.FirstOrDefaultAsync(a => a.Id == productId);
            if (product == null || quantity < 1) { return NotFound(); }
            var user = await GetCurrentUserAsync();
            if (user == null) { return NotFound(); }
            var cart = await _context.Carts.Include(a => a.CartProducts).ThenInclude(a => a.Product).FirstOrDefaultAsync(a => a.CustomerId == user.Id);
            if(cart == null) cart = await CreateCartForUser(user.Id);
            cart.CartProducts.Add(new ProductOrder<Cart> { ProductId = productId, Quantity = quantity, TotalPrice = product.SellingPrice * quantity});
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteItem(Guid cartProductId) 
        {
            var cartProduct = await _context.CartProducts.FirstOrDefaultAsync(a=>a.Id == cartProductId);
            if (cartProduct != null) 
            {
                _context.CartProducts.Remove(cartProduct);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
