using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Backend.Data;

public class ImageRepository(IWebHostEnvironment environment) : IImageRepository
{
    private readonly IWebHostEnvironment _environment = environment;
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
            return Path.Combine(_imageDirectory, fileName).Replace("\\", "/");
        }
        catch (Exception e)
        {

            throw new Exception("Error saving image", e);
        }
    }

    public void DeleteImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}