using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Backend.Tests.TestHelpers;

public static class TestDbContextFactory
{
    private static int _databaseCounter = 0;

    public static UserContext CreateInMemoryContext(string? databaseName = null)
    {
        var dbName = databaseName ?? $"TestDatabase_{Interlocked.Increment(ref _databaseCounter)}_{Guid.NewGuid():N}";
        var options = new DbContextOptionsBuilder<UserContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new UserContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static async Task<UserContext> CreateContextWithSeedDataAsync(string databaseName = "TestDatabase")
    {
        var context = CreateInMemoryContext(databaseName);

        // Seed Users
        var users = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        // Seed Books
        var books = new List<Book>
        {
            new Book
            {
                BookId = 1,
                BookName = "Test Book 1",
                Author = "Test Author 1",
                Category = "Fiction",
                Price = 19.99m,
                Description = "A test book for unit testing",
                ImageUrl = "https://example.com/book1.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Book
            {
                BookId = 2,
                BookName = "Test Book 2",
                Author = "Test Author 2",
                Category = "Non-Fiction",
                Price = 29.99m,
                Description = "Another test book for unit testing",
                ImageUrl = "https://example.com/book2.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Book
            {
                BookId = 3,
                BookName = "Programming Guide",
                Author = "Tech Author",
                Category = "Technology",
                Price = 39.99m,
                Description = "A comprehensive programming guide",
                ImageUrl = "https://example.com/programming.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();

        return context;
    }
}
