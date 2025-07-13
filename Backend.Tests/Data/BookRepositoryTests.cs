using Backend.Data;
using Backend.Models;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Backend.Tests.Data;

public class BookRepositoryTests : IDisposable
{
    private readonly UserContext _context;
    private readonly BookRepository _bookRepository;

    public BookRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext($"BookRepositoryTests_{Guid.NewGuid()}");
        _bookRepository = new BookRepository(_context);
        TestDataFactory.ResetCounters(); // Reset counters for each test
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateBookAsync_Should_AddBookToDatabase_When_ValidBookProvided()
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(
            id: 0, // Will be set by EF
            bookName: "New Test Book",
            price: 25.99m
        );

        // Act
        var result = await _bookRepository.CreateBookAsync(book);

        // Assert
        result.Should().NotBeNull();
        result.BookId.Should().BeGreaterThan(0);
        result.BookName.Should().Be("New Test Book");
        result.Price.Should().Be(25.99m);

        var bookInDb = await _context.Books.FindAsync(result.BookId);
        bookInDb.Should().NotBeNull();
        bookInDb!.BookName.Should().Be("New Test Book");
    }

    [Fact]
    public async Task CreateBookAsync_Should_SetCreatedDateAndNullUpdatedDate_When_BookCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var book = TestDataFactory.CreateTestBook(bookName: "Time Test Book");

        // Act
        var result = await _bookRepository.CreateBookAsync(book);

        // Assert
        result.CreatedAt.Should().BeAfter(beforeCreation);
        result.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        result.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task GetBookByIdAsync_Should_ReturnBook_When_BookExists()
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(bookName: "Existing Book");
        var createdBook = await _bookRepository.CreateBookAsync(book);

        // Act
        var result = await _bookRepository.GetBookByIdAsync(createdBook.BookId);

        // Assert
        result.Should().NotBeNull();
        result!.BookId.Should().Be(createdBook.BookId);
        result.BookName.Should().Be("Existing Book");
    }

    [Fact]
    public async Task GetBookByIdAsync_Should_ReturnNull_When_BookDoesNotExist()
    {
        // Act
        var result = await _bookRepository.GetBookByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task GetBookByIdAsync_Should_ReturnNull_When_InvalidIdProvided(int invalidId)
    {
        // Act
        var result = await _bookRepository.GetBookByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllBooksAsync_Should_ReturnAllBooks_When_BooksExist()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(3);
        foreach (var book in books)
        {
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var result = await _bookRepository.GetAllBooksAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(b => b.BookName == "Test Book 1");
        result.Should().Contain(b => b.BookName == "Test Book 2");
        result.Should().Contain(b => b.BookName == "Test Book 3");
    }

    [Fact]
    public async Task GetAllBooksAsync_Should_ReturnEmptyList_When_NoBooksExist()
    {
        // Act
        var result = await _bookRepository.GetAllBooksAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllBooksAsync_Should_ReturnBooksOrderedByCreatedDateDescending()
    {
        // Arrange
        var book1 = TestDataFactory.CreateTestBook(bookName: "First Book");
        book1.CreatedAt = DateTime.UtcNow.AddDays(-3);

        var book2 = TestDataFactory.CreateTestBook(bookName: "Second Book");
        book2.CreatedAt = DateTime.UtcNow.AddDays(-1);

        var book3 = TestDataFactory.CreateTestBook(bookName: "Third Book");
        book3.CreatedAt = DateTime.UtcNow;

        await _bookRepository.CreateBookAsync(book1);
        await _bookRepository.CreateBookAsync(book2);
        await _bookRepository.CreateBookAsync(book3);

        // Act
        var result = await _bookRepository.GetAllBooksAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].BookName.Should().Be("Third Book");  // Most recent
        result[1].BookName.Should().Be("Second Book");
        result[2].BookName.Should().Be("First Book");  // Oldest
    }

    [Fact]
    public async Task UpdateBookAsync_Should_UpdateBookProperties_When_ValidBookProvided()
    {
        // Arrange
        var originalBook = TestDataFactory.CreateTestBook(bookName: "Original Book", price: 20.00m);
        var createdBook = await _bookRepository.CreateBookAsync(originalBook);

        // Modify the book
        createdBook.BookName = "Updated Book";
        createdBook.Price = 30.00m;
        createdBook.Author = "Updated Author";
        createdBook.Category = "Updated Category";
        createdBook.Description = "Updated Description";

        // Act
        await _bookRepository.UpdateBookAsync(createdBook);

        // Assert
        var updatedBook = await _bookRepository.GetBookByIdAsync(createdBook.BookId);
        updatedBook.Should().NotBeNull();
        updatedBook!.BookName.Should().Be("Updated Book");
        updatedBook.Price.Should().Be(30.00m);
        updatedBook.Author.Should().Be("Updated Author");
        updatedBook.Category.Should().Be("Updated Category");
        updatedBook.Description.Should().Be("Updated Description");
        updatedBook.UpdatedAt.Should().NotBeNull();
        updatedBook.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task UpdateBookAsync_Should_ThrowInvalidOperationException_When_BookDoesNotExist()
    {
        // Arrange
        var nonExistentBook = TestDataFactory.CreateTestBook(id: 999, bookName: "Non-existent Book");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _bookRepository.UpdateBookAsync(nonExistentBook));
    }

    [Fact]
    public async Task UpdateBookAsync_Should_PreserveCreatedDate_When_BookUpdated()
    {
        // Arrange
        var originalBook = TestDataFactory.CreateTestBook(bookName: "Date Test Book");
        var createdBook = await _bookRepository.CreateBookAsync(originalBook);
        var originalCreatedAt = createdBook.CreatedAt;

        // Wait a moment to ensure different timestamps
        await Task.Delay(100);

        // Modify the book
        createdBook.BookName = "Updated Date Test Book";

        // Act
        await _bookRepository.UpdateBookAsync(createdBook);

        // Assert
        var updatedBook = await _bookRepository.GetBookByIdAsync(createdBook.BookId);
        updatedBook!.CreatedAt.Should().Be(originalCreatedAt);
        updatedBook.UpdatedAt.Should().NotBeNull();
        updatedBook.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public async Task DeleteBookAsync_Should_RemoveBookFromDatabase_When_BookExists()
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(bookName: "Book To Delete");
        var createdBook = await _bookRepository.CreateBookAsync(book);

        // Act
        await _bookRepository.DeleteBookAsync(createdBook.BookId);

        // Assert
        var deletedBook = await _bookRepository.GetBookByIdAsync(createdBook.BookId);
        deletedBook.Should().BeNull();
    }

    [Fact]
    public async Task DeleteBookAsync_Should_NotThrowException_When_BookDoesNotExist()
    {
        // Act & Assert
        await _bookRepository.DeleteBookAsync(999); // Should not throw
    }

    [Fact]
    public async Task SearchBooksAsync_Should_ReturnMatchingBooks_When_SearchTermMatches()
    {
        // Arrange
        var books = new[]
        {
            TestDataFactory.CreateTestBook(bookName: "Programming in C#"),
            TestDataFactory.CreateTestBook(bookName: "Java for Beginners"),
            TestDataFactory.CreateTestBook(bookName: "Web Development Guide")
        };

        books[0].Author = "John Programming";
        books[0].Category = "Technology";
        books[0].Description = "Learn C# programming";

        books[1].Author = "Jane Developer";
        books[1].Category = "Programming";
        books[1].Description = "Java basics";

        books[2].Author = "Web Master";
        books[2].Category = "Technology";
        books[2].Description = "HTML, CSS, JavaScript";

        foreach (var book in books)
        {
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var result = await _bookRepository.SearchBooksAsync("programming");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.BookName == "Programming in C#");
        result.Should().Contain(b => b.Author == "Jane Developer"); // Category contains "Programming"
    }

    [Fact]
    public async Task SearchBooksAsync_Should_ReturnEmptyList_When_NoMatchFound()
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(bookName: "Unrelated Book");
        await _bookRepository.CreateBookAsync(book);

        // Act
        var result = await _bookRepository.SearchBooksAsync("nonexistent");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchBooksAsync_Should_ReturnAllBooks_When_SearchTermIsEmpty()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(3);
        foreach (var book in books)
        {
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var result = await _bookRepository.SearchBooksAsync("");

        // Assert
        result.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("PROGRAMMING")]
    [InlineData("Programming")]
    [InlineData("programming")]
    [InlineData("PrOgRaMmInG")]
    public async Task SearchBooksAsync_Should_BeCaseInsensitive_When_SearchingBooks(string searchTerm)
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(bookName: "Programming Guide");
        await _bookRepository.CreateBookAsync(book);

        // Act
        var result = await _bookRepository.SearchBooksAsync(searchTerm);

        // Assert
        result.Should().HaveCount(1);
        result[0].BookName.Should().Be("Programming Guide");
    }

    [Fact]
    public async Task SearchBooksAsync_Should_SearchInAllFields_When_SearchTermProvided()
    {
        // Arrange
        var books = new[]
        {
            new Book { BookName = "Test in Title", Author = "Author", Category = "Category", Description = "Description" },
            new Book { BookName = "Book", Author = "Test in Author", Category = "Category", Description = "Description" },
            new Book { BookName = "Book", Author = "Author", Category = "Test in Category", Description = "Description" },
            new Book { BookName = "Book", Author = "Author", Category = "Category", Description = "Test in Description" }
        };

        foreach (var book in books)
        {
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var result = await _bookRepository.SearchBooksAsync("test");

        // Assert
        result.Should().HaveCount(4);
    }

    [Fact]
    public async Task SearchBooksAsync_WithPagination_Should_ReturnCorrectPageSize_When_ParametersProvided()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(10);
        foreach (var book in books)
        {
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var result = await _bookRepository.SearchBooksAsync("Test", page: 1, pageSize: 3);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task SearchBooksAsync_WithPagination_Should_ReturnCorrectPage_When_PageNumberProvided()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(5);
        foreach (var book in books)
        {
            book.CreatedAt = DateTime.UtcNow.AddDays(-book.BookId); // Different dates for ordering
            await _bookRepository.CreateBookAsync(book);
        }

        // Act
        var page1 = await _bookRepository.SearchBooksAsync("Test", page: 1, pageSize: 2);
        var page2 = await _bookRepository.SearchBooksAsync("Test", page: 2, pageSize: 2);

        // Assert
        page1.Should().HaveCount(2);
        page2.Should().HaveCount(2);

        // Ensure different books on different pages
        var page1Ids = page1.Select(b => b.BookId).ToList();
        var page2Ids = page2.Select(b => b.BookId).ToList();
        page1Ids.Should().NotIntersectWith(page2Ids);
    }
}
