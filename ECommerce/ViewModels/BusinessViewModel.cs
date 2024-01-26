using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class BusinessViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public Guid BusinessCategoryId { get; set; }
    }
}
