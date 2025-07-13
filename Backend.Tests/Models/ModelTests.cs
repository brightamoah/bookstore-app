using Backend.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Backend.Tests.Models;

public class BookTests
{
    [Fact]
    public void Book_Should_InitializeWithDefaultValues_When_Created()
    {
        // Act
        var book = new Book();

        // Assert
        book.BookId.Should().Be(0);
        book.BookName.Should().Be(string.Empty);
        book.Category.Should().Be(string.Empty);
        book.Author.Should().Be(string.Empty);
        book.Price.Should().Be(0);
        book.Description.Should().Be(string.Empty);
        book.ImageUrl.Should().Be(string.Empty);
        book.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        book.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Book_Should_SetPropertiesCorrectly_When_ValuesProvided()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var book = new Book
        {
            BookId = 1,
            BookName = "Test Book",
            Category = "Fiction",
            Author = "Test Author",
            Price = 19.99m,
            Description = "A test book description",
            ImageUrl = "https://example.com/book.jpg",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1)
        };

        // Assert
        book.BookId.Should().Be(1);
        book.BookName.Should().Be("Test Book");
        book.Category.Should().Be("Fiction");
        book.Author.Should().Be("Test Author");
        book.Price.Should().Be(19.99m);
        book.Description.Should().Be("A test book description");
        book.ImageUrl.Should().Be("https://example.com/book.jpg");
        book.CreatedAt.Should().Be(now);
        book.UpdatedAt.Should().Be(now.AddDays(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.01)]
    [InlineData(9999.99)]
    public void Book_Should_AcceptValidPrices_When_PriceIsValid(decimal price)
    {
        // Arrange & Act
        var book = new Book { Price = price };

        // Assert
        book.Price.Should().Be(price);
    }

    [Fact]
    public void Book_Should_HandleNullUpdateDate_When_NotUpdated()
    {
        // Arrange & Act
        var book = new Book
        {
            BookName = "Test Book",
            UpdatedAt = null
        };

        // Assert
        book.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Book_CreatedAt_Should_BeSetAutomatically_When_BookCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var book = new Book();

        // Assert
        book.CreatedAt.Should().BeAfter(beforeCreation);
        book.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }
}

public class UserTests
{
    [Fact]
    public void User_Should_InitializeWithDefaultValues_When_Created()
    {
        // Act
        var user = new User
        {
            Name = "Required Name",
            Email = "required@email.com"
        };

        // Assert
        user.Id.Should().Be(0);
        user.Name.Should().Be("Required Name");
        user.Email.Should().Be("required@email.com");
        user.PhoneNumber.Should().BeNull();
        user.Address.Should().BeNull();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeNull();
        user.Password.Should().Be(string.Empty);
    }

    [Fact]
    public void User_Should_SetPropertiesCorrectly_When_ValuesProvided()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Address = "123 Main St",
            CreatedAt = now,
            UpdatedAt = now.AddDays(1),
            Password = "hashedpassword"
        };

        // Assert
        user.Id.Should().Be(1);
        user.Name.Should().Be("John Doe");
        user.Email.Should().Be("john@example.com");
        user.PhoneNumber.Should().Be("1234567890");
        user.Address.Should().Be("123 Main St");
        user.CreatedAt.Should().Be(now);
        user.UpdatedAt.Should().Be(now.AddDays(1));
        user.Password.Should().Be("hashedpassword");
    }

    [Fact]
    public void User_CreatedAt_Should_BeSetAutomatically_When_UserCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var user = new User
        {
            Name = "Test User",
            Email = "test@example.com"
        };

        // Assert
        user.CreatedAt.Should().BeAfter(beforeCreation);
        user.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public void User_Should_AllowValidConstruction_When_CreatedWithRequiredProperties()
    {
        // This test verifies that the required properties can be set correctly

        // Act
        var user = new User { Name = "Test", Email = "test@example.com" };

        // Assert
        user.Name.Should().Be("Test");
        user.Email.Should().Be("test@example.com");
    }
}

public class ErrorResponseTests
{
    [Fact]
    public void ErrorResponse_Should_InitializeWithDefaultValues_When_Created()
    {
        // Act
        var errorResponse = new ErrorResponse();

        // Assert
        errorResponse.Message.Should().Be(string.Empty);
        errorResponse.Details.Should().BeNull();
        errorResponse.ErrorCode.Should().Be(string.Empty);
        errorResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        errorResponse.TraceId.Should().BeNull();
        errorResponse.ValidationErrors.Should().BeNull();
    }

    [Fact]
    public void ErrorResponse_Should_SetPropertiesCorrectly_When_ValuesProvided()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;
        var errorResponse = new ErrorResponse
        {
            Message = "Test error message",
            Details = "Error details",
            ErrorCode = "TEST_ERROR",
            Timestamp = timestamp,
            TraceId = "trace-123",
            ValidationErrors = new { Field = "Error" }
        };

        // Assert
        errorResponse.Message.Should().Be("Test error message");
        errorResponse.Details.Should().Be("Error details");
        errorResponse.ErrorCode.Should().Be("TEST_ERROR");
        errorResponse.Timestamp.Should().Be(timestamp);
        errorResponse.TraceId.Should().Be("trace-123");
        errorResponse.ValidationErrors.Should().NotBeNull();
    }

    [Fact]
    public void ValidationErrorResponse_Should_InheritFromErrorResponse()
    {
        // Act
        var validationError = new ValidationErrorResponse();

        // Assert
        validationError.Should().BeAssignableTo<ErrorResponse>();
        validationError.Errors.Should().NotBeNull();
        validationError.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationErrorResponse_Should_SetErrorsDictionary_When_ValuesProvided()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "Name", new[] { "Name is required" } },
            { "Email", new[] { "Email is invalid", "Email is required" } }
        };

        // Act
        var validationError = new ValidationErrorResponse
        {
            Errors = errors
        };

        // Assert
        validationError.Errors.Should().HaveCount(2);
        validationError.Errors["Name"].Should().ContainSingle("Name is required");
        validationError.Errors["Email"].Should().HaveCount(2);
    }
}

public class ErrorCodesTests
{
    [Fact]
    public void ErrorCodes_Should_HaveConsistentAuthenticationCodes()
    {
        // Assert
        ErrorCodes.UNAUTHORIZED.Should().Be("AUTH_001");
        ErrorCodes.INVALID_CREDENTIALS.Should().Be("AUTH_002");
        ErrorCodes.TOKEN_EXPIRED.Should().Be("AUTH_003");
        ErrorCodes.ACCESS_DENIED.Should().Be("AUTH_004");
        ErrorCodes.USER_ALREADY_EXISTS.Should().Be("AUTH_005");
        ErrorCodes.WEAK_PASSWORD.Should().Be("AUTH_006");
        ErrorCodes.USER_NOT_FOUND.Should().Be("AUTH_007");
    }

    [Fact]
    public void ErrorCodes_Should_HaveConsistentBookCodes()
    {
        // Assert
        ErrorCodes.BOOK_NOT_FOUND.Should().Be("BOOK_001");
        ErrorCodes.BOOK_ALREADY_EXISTS.Should().Be("BOOK_002");
        ErrorCodes.INVALID_BOOK_DATA.Should().Be("BOOK_003");
        ErrorCodes.BOOK_CREATION_FAILED.Should().Be("BOOK_004");
        ErrorCodes.BOOK_UPDATE_FAILED.Should().Be("BOOK_005");
        ErrorCodes.BOOK_DELETE_FAILED.Should().Be("BOOK_006");
    }

    [Fact]
    public void ErrorCodes_Should_HaveUniqueValues()
    {
        // Arrange
        var codeValues = new[]
        {
            ErrorCodes.UNAUTHORIZED,
            ErrorCodes.INVALID_CREDENTIALS,
            ErrorCodes.TOKEN_EXPIRED,
            ErrorCodes.ACCESS_DENIED,
            ErrorCodes.USER_ALREADY_EXISTS,
            ErrorCodes.WEAK_PASSWORD,
            ErrorCodes.USER_NOT_FOUND,
            ErrorCodes.BOOK_NOT_FOUND,
            ErrorCodes.BOOK_ALREADY_EXISTS,
            ErrorCodes.INVALID_BOOK_DATA,
            ErrorCodes.BOOK_CREATION_FAILED,
            ErrorCodes.BOOK_UPDATE_FAILED,
            ErrorCodes.BOOK_DELETE_FAILED
        };

        // Assert
        codeValues.Should().OnlyHaveUniqueItems();
    }
}
