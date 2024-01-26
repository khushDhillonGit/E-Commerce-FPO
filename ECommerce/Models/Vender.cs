using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Vender
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public Guid AddressId { get; set; }
        [ForeignKey(nameof(AddressId))]
        public virtual Address Address { get; set; }
        public Guid BusinessId { get; set; }
        [ForeignKey(nameof(BusinessId))]
        public virtual Business Business { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public bool IsDelete { get; set; }
    }
}
