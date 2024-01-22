using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class CreateBusinessViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public Guid BusinessCategoryId { get; set; }
        public IFormFile? Image { get; set; }
        [Required]
        public string? StreetAddress { get; set; }
        public string? UnitApt { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Province { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? Country { get; set; }
    }
}
