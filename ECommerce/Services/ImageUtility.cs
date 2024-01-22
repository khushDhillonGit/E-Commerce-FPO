namespace ECommerce.Services
{
    public class ImageUtility
    {
        private readonly IWebHostEnvironment _environment;
        public ImageUtility(IWebHostEnvironment environment) 
        {
            _environment = environment;
        }

        public async Task<string> SaveImageToServerAsync(IFormFile image, string path)
        {
            var imagePath = Path.Combine(path, Guid.NewGuid().ToString() + "_" + image.FileName);
            if (_environment == null) throw new ArgumentNullException("Root path not found");
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return imagePath;
        }
    }
}
