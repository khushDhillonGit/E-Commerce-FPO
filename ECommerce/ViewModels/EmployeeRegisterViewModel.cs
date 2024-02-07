using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class EmployeeRegisterViewModel : AddressViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        public Guid BusinessId { get; set; }
        public SelectList? BusinessesList { get; set; }
    }
}
