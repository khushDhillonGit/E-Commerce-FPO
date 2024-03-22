using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Models
{
    public class CustomerOrder
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<ProductOrder<CustomerOrder>> CustomerOrderProducts { get; set; } = new List<ProductOrder<CustomerOrder>>();
        public string CustomerName { get; set; } = string.Empty;   
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalBill { get; set; }
        public DateTimeOffset OrderDate { get; set; }

    }
}
