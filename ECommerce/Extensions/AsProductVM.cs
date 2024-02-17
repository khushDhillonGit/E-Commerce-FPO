using ECommerce.ViewModels;
using ECommerce.Models;
using AutoMapper;

namespace ECommerce.Extensions
{
    public static class Extensions 
    {
        public static ProductOnSaleViewModel AsProductOnSaleVM(this Product product) 
        {
            return new ProductOnSaleViewModel 
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name,
                BusinessName = product.Category?.Business?.Name
            };
        }
    }
}
