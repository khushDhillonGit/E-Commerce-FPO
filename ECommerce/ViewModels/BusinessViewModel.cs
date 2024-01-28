using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class BusinessViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public Guid BusinessCategoryId { get; set; }
        public AddressViewModel Address { get; set; } = new AddressViewModel();
        public SelectList? Categories { get; set; }
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalOrders { get; set; }

    }
}
