using Backend.DataTransferObjects;
using Backend.Models;

namespace Backend.Tests.TestHelpers;

public static class TestDataFactory
{
    private static int _bookCounter = 0;
    private static int _userCounter = 0;

    public static Book CreateTestBook(int? id = null, string? bookName = null, string? author = null, decimal? price = null)
    {
        var counter = ++_bookCounter;
        return new Book
        {
            BookId = id ?? 0, // Let EF assign the ID for new entities
            BookName = bookName ?? $"Test Book {counter}",
            Author = author ?? $"Test Author {counter}",
            Category = $"Test Category {counter}",
            Description = $"Test Description {counter}",
            Price = price ?? 19.99m,
            ImageUrl = $"https://example.com/book-{counter}.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static List<Book> CreateTestBooks(int count)
    {
        var books = new List<Book>();
        for (int i = 0; i < count; i++)
        {
            books.Add(CreateTestBook());
        }
        return books;
    }

    public static User CreateTestUser(int? id = null, string? name = null, string? email = null, string? password = null)
    {
        var counter = ++_userCounter;
        return new User
        {
            Id = id ?? 0, // Let EF assign the ID for new entities
            Name = name ?? $"Test User {counter}",
            Email = email ?? $"test{counter}@example.com",
            Password = password ?? "hashedpassword",
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<User> CreateTestUsers(int count)
    {
        var users = new List<User>();
        for (int i = 0; i < count; i++)
        {
            users.Add(CreateTestUser());
        }
        return users;
    }

    public static BookDto CreateTestBookDto(int? id = null, string? bookName = null, string? author = null, decimal? price = null)
    {
        var counter = ++_bookCounter;
        return new BookDto
        {
            BookId = id ?? 0, // Let EF assign the ID for new entities
            BookName = bookName ?? $"Test Book {counter}",
            Author = author ?? $"Test Author {counter}",
            Category = $"Test Category {counter}",
            Description = $"Test Description {counter}",
            Price = price ?? 19.99m,
            ImageUrl = $"https://example.com/book-{counter}.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };
    }

    public static void ResetCounters()
    {
        _bookCounter = 0;
        _userCounter = 0;
    }
}
