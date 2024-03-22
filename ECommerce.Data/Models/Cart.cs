using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Data.Models
{
    public class Cart
    {
        [Key]
        public Guid Id { get; set; }
        public virtual ICollection<ProductOrder<Cart>> CartProducts { get; set; } = new List<ProductOrder<Cart>>();
        public Guid CustomerId { get; set; }
        [ForeignKey(nameof(CustomerId))]
        public virtual ApplicationUser? ApplicationUser { get; set; }

    }
}
