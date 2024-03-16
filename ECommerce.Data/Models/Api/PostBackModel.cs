
namespace ECommerce.Data.Models.Api
{
    public class PostBackModel
    {
        public bool Success { get; set; }
        public string? ResponseText { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
