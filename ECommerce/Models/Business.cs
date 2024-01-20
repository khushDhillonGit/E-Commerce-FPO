using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Business
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? Phone { get; set; }
        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; }    
        public Guid OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public ApplicationUser Owner { get; set; }
    }
}
