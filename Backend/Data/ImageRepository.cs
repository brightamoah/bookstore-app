using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Backend.Data;

public class ImageRepository(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : IImageRepository
{
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly string _imageDirectory = "uploads/books";

    public async Task<string> SaveImageAsync(string base64Image)
    {
        try
        {
            // Remove data:image/[type];base64, from the string
            var base64Data = base64Image[(base64Image.IndexOf(",") + 1)..];
            var imageBytes = Convert.FromBase64String(base64Data);

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, _imageDirectory);
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}.jpg";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Process and save the image
            using (var image = Image.Load(imageBytes))
            {
                // Resize if needed
                image.Mutate(x => x.Resize(800, 0)); // Maintain aspect ratio

                await image.SaveAsJpegAsync(filePath);
            }
            // Return the relative path
            var relativePath = Path.Combine(_imageDirectory, fileName).Replace("\\", "/");
            return GetFullImageUrl(relativePath);
        }
        catch (Exception e)
        {

            throw new Exception("Error saving image", e);
        }
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        // Handle both full URLs and relative paths
        var relativePath = imagePath.StartsWith("http")
            ? new Uri(imagePath).AbsolutePath.TrimStart('/')
            : imagePath;

        var fullPath = Path.Combine(_environment.WebRootPath, relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string GetFullImageUrl(string relativePath)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/{relativePath}";
        }

        // Fallback for when HttpContext is not available
        return $"/{relativePath}";
    }
}