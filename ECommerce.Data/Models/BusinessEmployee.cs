using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Data.Models
{
    public class BusinessEmployee
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual ApplicationUser Employee { get; set; }   
        public Guid BusinessId { get; set; }
        [ForeignKey(nameof(BusinessId))]
        public virtual Business Business { get; set; }   
    }
}
