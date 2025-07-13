using Backend.Data;
using Backend.Models;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Backend.Tests.Integration;

public class BookStoreIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly UserContext _context;

    public BookStoreIntegrationTests(WebApplicationFactory<Program> factory)
    {
        var databaseName = $"IntegrationTestDb_{Guid.NewGuid()}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<UserContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database for testing with unique name
                services.AddDbContext<UserContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName);
                });
            });
        });

        _client = _factory.CreateClient();

        // Get the in-memory database context
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<UserContext>();
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }

    #region Authentication Flow Tests

    [Fact]
    public async Task AuthFlow_Should_AllowSignupLoginAndGetUser_When_ValidCredentialsProvided()
    {
        // 1. Signup
        var signupRequest = new
        {
            Name = "Integration Test User",
            Email = "integration@test.com",
            Password = "password123"
        };

        var signupResponse = await _client.PostAsJsonAsync("/api/signup", signupRequest);
        signupResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // 2. Login
        var loginRequest = new
        {
            Email = "integration@test.com",
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/login", loginRequest);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Extract JWT token from cookies
        var cookies = loginResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();
        cookies.Should().Contain("jwtToken=");

        // 3. Get current user (with JWT cookie)
        _client.DefaultRequestHeaders.Add("Cookie", cookies);
        var userResponse = await _client.GetAsync("/api/user");
        userResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var userContent = await userResponse.Content.ReadAsStringAsync();
        var userData = JsonSerializer.Deserialize<JsonElement>(userContent);
        userData.GetProperty("name").GetString().Should().Be("Integration Test User");
        userData.GetProperty("email").GetString().Should().Be("integration@test.com");
    }

    [Fact]
    public async Task AuthFlow_Should_PreventDuplicateSignup_When_EmailAlreadyExists()
    {
        // 1. First signup
        var signupRequest = new
        {
            Name = "First User",
            Email = "duplicate@test.com",
            Password = "password123"
        };

        var firstSignupResponse = await _client.PostAsJsonAsync("/api/signup", signupRequest);
        firstSignupResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // 2. Second signup with same email
        var duplicateSignupRequest = new
        {
            Name = "Second User",
            Email = "duplicate@test.com",
            Password = "differentpassword"
        };

        var secondSignupResponse = await _client.PostAsJsonAsync("/api/signup", duplicateSignupRequest);
        secondSignupResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_InvalidCredentialsProvided()
    {
        // Arrange - Create a user first
        var user = TestDataFactory.CreateTestUser(email: "validuser@test.com");
        user.Password = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act - Try to login with wrong password
        var loginRequest = new
        {
            Email = "validuser@test.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Books API Tests

    [Fact]
    public async Task BooksAPI_Should_RequireAuthentication_When_AccessingProtectedEndpoints()
    {
        // Test GET /api/books without authentication
        var response = await _client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task BooksAPI_Should_AllowCRUDOperations_When_Authenticated()
    {
        // Setup authentication
        var jwtToken = await AuthenticateUser();
        _client.DefaultRequestHeaders.Add("Cookie", $"jwtToken={jwtToken}");

        // 1. Create a book
        var createBookRequest = new
        {
            BookName = "Integration Test Book",
            Category = "Test Category",
            Author = "Test Author",
            Price = 29.99,
            Description = "A book for integration testing",
            ImageUrl = "https://example.com/book.jpg"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/books", createBookRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBook = JsonSerializer.Deserialize<JsonElement>(createContent);
        var bookId = createdBook.GetProperty("bookId").GetInt32();

        // 2. Get all books
        var getAllResponse = await _client.GetAsync("/api/books");
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAllContent = await getAllResponse.Content.ReadAsStringAsync();
        var allBooks = JsonSerializer.Deserialize<JsonElement[]>(getAllContent);
        allBooks.Should().HaveCountGreaterThan(0);

        // 3. Get book by ID
        var getByIdResponse = await _client.GetAsync($"/api/books/{bookId}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getByIdContent = await getByIdResponse.Content.ReadAsStringAsync();
        var retrievedBook = JsonSerializer.Deserialize<JsonElement>(getByIdContent);
        retrievedBook.GetProperty("bookName").GetString().Should().Be("Integration Test Book");

        // 4. Update the book
        var updateBookRequest = new
        {
            BookId = bookId,
            BookName = "Updated Integration Test Book",
            Category = "Updated Category",
            Author = "Updated Author",
            Price = 39.99,
            Description = "Updated description",
            ImageUrl = "https://example.com/updated-book.jpg",
            CreatedAt = createdBook.GetProperty("createdAt").GetString(),
            UpdatedAt = DateTime.UtcNow.ToString("O")
        };

        var updateResponse = await _client.PutAsJsonAsync($"/api/books/{bookId}", updateBookRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 5. Search for books
        var searchResponse = await _client.GetAsync("/api/books/search?query=Updated");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<JsonElement>(searchContent);
        var searchBooks = searchResult.GetProperty("books");
        searchBooks.GetArrayLength().Should().BeGreaterThan(0);

        // 6. Delete the book
        var deleteResponse = await _client.DeleteAsync($"/api/books/{bookId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 7. Verify book is deleted
        var getDeletedResponse = await _client.GetAsync($"/api/books/{bookId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task BooksAPI_Should_HandleNotFound_When_BookDoesNotExist()
    {
        // Setup authentication
        var jwtToken = await AuthenticateUser();
        _client.DefaultRequestHeaders.Add("Cookie", $"jwtToken={jwtToken}");

        // Try to get non-existent book
        var response = await _client.GetAsync("/api/books/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var content = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonSerializer.Deserialize<JsonElement>(content);
        errorResponse.GetProperty("errorCode").GetString().Should().Be("BOOK_001");
    }

    [Fact]
    public async Task BooksAPI_Should_HandleValidationErrors_When_InvalidDataProvided()
    {
        // Setup authentication
        var jwtToken = await AuthenticateUser();
        _client.DefaultRequestHeaders.Add("Cookie", $"jwtToken={jwtToken}");

        // Try to create book with missing required fields
        var invalidBookRequest = new
        {
            BookName = "", // Invalid - empty
            Category = "Test",
            Author = "", // Invalid - empty
            Price = -10, // Invalid - negative
            Description = "Test",
            ImageUrl = "https://example.com/test.jpg" // Valid URL to test other validation rules
        };

        var response = await _client.PostAsJsonAsync("/api/books", invalidBookRequest);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Search Tests

    [Fact]
    public async Task SearchAPI_Should_ReturnFilteredResults_When_QueryProvided()
    {
        // Setup authentication
        var jwtToken = await AuthenticateUser();
        _client.DefaultRequestHeaders.Add("Cookie", $"jwtToken={jwtToken}");

        // Create test books
        var books = new[]
        {
            new { BookName = "C# Programming Guide", Category = "Programming", Author = "Microsoft", Price = 49.99, Description = "Learn C# programming", ImageUrl = "https://example.com/csharp.jpg" },
            new { BookName = "Java Basics", Category = "Programming", Author = "Oracle", Price = 39.99, Description = "Java fundamentals", ImageUrl = "https://example.com/java.jpg" },
            new { BookName = "Cooking for Beginners", Category = "Cooking", Author = "Chef Smith", Price = 19.99, Description = "Basic cooking skills", ImageUrl = "https://example.com/cooking.jpg" }
        };

        foreach (var book in books)
        {
            await _client.PostAsJsonAsync("/api/books", book);
        }

        // Search for programming books
        var searchResponse = await _client.GetAsync("/api/books/search?query=programming");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var searchContent = await searchResponse.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<JsonElement>(searchContent);
        var foundBooks = searchResult.GetProperty("books");

        foundBooks.GetArrayLength().Should().Be(2); // Should find 2 programming books
    }

    [Fact]
    public async Task SearchAPI_Should_HandlePagination_When_ParametersProvided()
    {
        // Setup authentication
        var jwtToken = await AuthenticateUser();
        _client.DefaultRequestHeaders.Add("Cookie", $"jwtToken={jwtToken}");

        // Create multiple test books
        for (int i = 1; i <= 5; i++)
        {
            var book = new
            {
                BookName = $"Test Book {i}",
                Category = "Test",
                Author = $"Author {i}",
                Price = 10.0 + i,
                Description = $"Description for book {i}",
                ImageUrl = $"https://example.com/book{i}.jpg"
            };
            await _client.PostAsJsonAsync("/api/books", book);
        }

        // Test pagination
        var pageResponse = await _client.GetAsync("/api/books/search?query=Test&page=1&pageSize=2");
        pageResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var pageContent = await pageResponse.Content.ReadAsStringAsync();
        var pageResult = JsonSerializer.Deserialize<JsonElement>(pageContent);

        pageResult.GetProperty("page").GetInt32().Should().Be(1);
        pageResult.GetProperty("pageSize").GetInt32().Should().Be(2);
    }

    #endregion

    #region Helper Methods

    private async Task<string> AuthenticateUser()
    {
        // Create a test user
        var signupRequest = new
        {
            Name = "Test User",
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "password123"
        };

        await _client.PostAsJsonAsync("/api/signup", signupRequest);

        // Login to get JWT token
        var loginRequest = new
        {
            Email = signupRequest.Email,
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/login", loginRequest);
        var cookies = loginResponse.Headers.GetValues("Set-Cookie").FirstOrDefault();

        // Extract token from cookie
        var tokenStart = cookies!.IndexOf("jwtToken=") + "jwtToken=".Length;
        var tokenEnd = cookies.IndexOf(";", tokenStart);
        if (tokenEnd == -1) tokenEnd = cookies.Length;

        return cookies.Substring(tokenStart, tokenEnd - tokenStart);
    }

    #endregion
}
