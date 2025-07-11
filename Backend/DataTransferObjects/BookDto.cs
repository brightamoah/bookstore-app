using System.ComponentModel.DataAnnotations;

namespace Backend.DataTransferObjects;

public class BookDto
{
    public int BookId { get; set; }
    [Required]
    [StringLength(200)]
    public required string BookName { get; set; }

    [Required]
    [StringLength(100)]
    public required string Category { get; set; }

    [Required]
    [Range(0.01, 9999.99)]
    public decimal Price { get; set; }

    [Required]
    [StringLength(2000)]
    public required string Description { get; set; }

    [Required]
    [Url]
    public required string ImageUrl { get; set; }

}