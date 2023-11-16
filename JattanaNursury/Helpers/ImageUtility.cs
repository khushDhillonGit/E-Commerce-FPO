using System.Drawing.Text;
using System.Security.Cryptography;

namespace JattanaNursury.Helpers
{
    public static class ImageUtility
    {
        public static async Task<string> SaveImageToServerAsync(IWebHostEnvironment webHostEnvironment, IFormFile image, string path)
        {
            var imagePath = Path.Combine(path, Guid.NewGuid().ToString() + "_" + image.FileName);
            if (webHostEnvironment == null) throw new ArgumentNullException("Root path not found");
            var fullPath = Path.Combine(webHostEnvironment.WebRootPath, imagePath);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return imagePath;
        }
    }
}
