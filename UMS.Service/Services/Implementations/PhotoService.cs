using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using UMS.Service.Services.Interfaces;

namespace UMS.Service.Services.Implementations
{
    public class PhotoService : IPhotoService
    {
        private readonly string _uploadsFolder;

        public PhotoService(IWebHostEnvironment env)
        {
            var webRoot = env.WebRootPath
                ?? Path.Combine(env.ContentRootPath, "wwwroot");

            _uploadsFolder = Path.Combine(webRoot, "uploads", "photos");
            Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"File type '{extension}' is not allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/photos/{fileName}";
        }

        public void DeletePhoto(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
                return;

            var fileName = Path.GetFileName(photoUrl);
            var filePath = Path.Combine(_uploadsFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
