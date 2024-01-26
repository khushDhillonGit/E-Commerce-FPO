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
            if (_environment == null) throw new ArgumentNullException("Root path not found");
            var directPath = Path.Combine(_environment.WebRootPath, path);
            if(!Directory.Exists(directPath)) 
                Directory.CreateDirectory(directPath);
            var imagePath = Path.Combine(path, Guid.NewGuid().ToString() + "_" + image.FileName);
            var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return imagePath;
        }
    }
}
