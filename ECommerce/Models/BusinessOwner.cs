namespace ECommerce.Models
{
    public class BusinessOwner
    {
        public BusinessOwner() 
        {
            Owners = new List<ApplicationUser>();
            Businesses = new List<Business>();
        }
        public Guid Id { get; set; }
        public ICollection<ApplicationUser> Owners { get; set; }
        public ICollection<Business> Businesses { get; set; }
    }
}
