using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class AddressViewModel
    {
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
