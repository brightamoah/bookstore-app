namespace Backend.Data;

public interface IImageRepository
{
    Task<string> SaveImageAsync(string base64Image);
    void DeleteImage(string imagePath);
}