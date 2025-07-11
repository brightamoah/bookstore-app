namespace Backend.Models;

public class Book
{

    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}