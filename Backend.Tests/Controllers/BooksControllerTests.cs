using Backend.Controllers;
using Backend.Data;
using Backend.Models;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Backend.Tests.Controllers;

public class BooksControllerTests : IDisposable
{
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IImageRepository> _mockImageRepository;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookRepository = new Mock<IBookRepository>();
        _mockImageRepository = new Mock<IImageRepository>();
        _controller = new BooksController(_mockBookRepository.Object, _mockImageRepository.Object);

        // Setup basic HttpContext with proper service provider
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _mockBookRepository.Object);
        serviceCollection.AddScoped(_ => _mockImageRepository.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var httpContext = ControllerTestHelper.CreateHttpContext(serviceProvider);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            ActionDescriptor = new ControllerActionDescriptor()
        };
    }

    public void Dispose()
    {
        _controller.Dispose();
    }

    #region GetAllBooks Tests

    [Fact]
    public async Task GetAllBooks_Should_ReturnOkWithBooks_When_BooksExist()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(3);
        _mockBookRepository.Setup(r => r.GetAllBooksAsync())
            .ReturnsAsync(books);

        // Act
        var result = await _controller.GetAllBooks();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(books);
    }

    [Fact]
    public async Task GetAllBooks_Should_ReturnOkWithEmptyList_When_NoBooksExist()
    {
        // Arrange
        _mockBookRepository.Setup(r => r.GetAllBooksAsync())
            .ReturnsAsync(new List<Book>());

        // Act
        var result = await _controller.GetAllBooks();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var books = okResult!.Value as List<Book>;
        books.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllBooks_Should_ReturnInternalServerError_When_ExceptionThrown()
    {
        // Arrange
        _mockBookRepository.Setup(r => r.GetAllBooksAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAllBooks();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INTERNAL_SERVER_ERROR);
    }

    #endregion

    #region GetBookById Tests

    [Fact]
    public async Task GetBookById_Should_ReturnOkWithBook_When_BookExists()
    {
        // Arrange
        var book = TestDataFactory.CreateTestBook(1, "Test Book");
        _mockBookRepository.Setup(r => r.GetBookByIdAsync(1))
            .ReturnsAsync(book);

        // Act
        var result = await _controller.GetBookById(1);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(book);
    }

    [Fact]
    public async Task GetBookById_Should_ReturnNotFound_When_BookDoesNotExist()
    {
        // Arrange
        _mockBookRepository.Setup(r => r.GetBookByIdAsync(999))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.GetBookById(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(404);

        var errorResponse = notFoundResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.BOOK_NOT_FOUND);
    }

    [Fact]
    public async Task GetBookById_Should_ReturnInternalServerError_When_ExceptionThrown()
    {
        // Arrange
        _mockBookRepository.Setup(r => r.GetBookByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetBookById(1);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INTERNAL_SERVER_ERROR);
    }

    #endregion

    #region CreateBook Tests

    [Fact]
    public async Task CreateBook_Should_ReturnCreatedResult_When_ValidBookProvided()
    {
        // Arrange
        var bookDto = TestDataFactory.CreateTestBookDto(0, "New Book");
        var createdBook = TestDataFactory.CreateTestBook(1, "New Book");

        _mockBookRepository.Setup(r => r.CreateBookAsync(It.IsAny<Book>()))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _controller.CreateBook(bookDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(createdBook);
    }

    [Fact]
    public async Task CreateBook_Should_ProcessImageUrl_When_Base64ImageProvided()
    {
        // Arrange
        var bookDto = TestDataFactory.CreateTestBookDto(0, "Book with Image");
        bookDto.ImageUrl = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAAAAAAAD";

        var createdBook = TestDataFactory.CreateTestBook(1, "Book with Image");
        createdBook.ImageUrl = "https://example.com/processed-image.jpg";

        _mockImageRepository.Setup(i => i.SaveImageAsync("data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAAAAAAAD"))
            .ReturnsAsync("https://example.com/processed-image.jpg");
        _mockBookRepository.Setup(r => r.CreateBookAsync(It.IsAny<Book>()))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _controller.CreateBook(bookDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        _mockImageRepository.Verify(i => i.SaveImageAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateBook_Should_ReturnBadRequest_When_ImageProcessingFails()
    {
        // Arrange
        var bookDto = TestDataFactory.CreateTestBookDto(0, "Book with Invalid Image");
        bookDto.ImageUrl = "data:image/jpeg;base64,invalid";

        _mockImageRepository.Setup(i => i.SaveImageAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Invalid image format"));

        // Act
        var result = await _controller.CreateBook(bookDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(400);

        var errorResponse = badRequestResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.IMAGE_PROCESSING_FAILED);
    }

    [Fact]
    public async Task CreateBook_Should_ReturnInternalServerError_When_CreateBookFails()
    {
        // Arrange
        var bookDto = TestDataFactory.CreateTestBookDto(0, "Failing Book");

        _mockBookRepository.Setup(r => r.CreateBookAsync(It.IsAny<Book>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateBook(bookDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.BOOK_CREATION_FAILED);
    }

    #endregion

    #region UpdateBook Tests

    [Fact]
    public async Task UpdateBook_Should_ReturnOkWithUpdatedBook_When_ValidUpdateProvided()
    {
        // Arrange
        var bookId = 1;
        var existingBook = TestDataFactory.CreateTestBook(bookId, "Original Book");
        var updatedBook = TestDataFactory.CreateTestBook(bookId, "Updated Book");
        updatedBook.Price = 29.99m;

        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(existingBook);
        _mockBookRepository.Setup(r => r.UpdateBookAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);
        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(updatedBook);

        // Act
        var result = await _controller.UpdateBook(bookId, updatedBook);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockBookRepository.Verify(r => r.UpdateBookAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_Should_ReturnBadRequest_When_IdMismatch()
    {
        // Arrange
        var urlId = 1;
        var book = TestDataFactory.CreateTestBook(2, "Mismatched Book"); // Different ID

        // Act
        var result = await _controller.UpdateBook(urlId, book);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(400);

        var errorResponse = badRequestResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INVALID_BOOK_DATA);
        errorResponse.Message.Should().Contain("ID mismatch");
    }

    [Fact]
    public async Task UpdateBook_Should_ReturnNotFound_When_BookDoesNotExist()
    {
        // Arrange
        var bookId = 999;
        var book = TestDataFactory.CreateTestBook(bookId, "Non-existent Book");

        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.UpdateBook(bookId, book);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(404);

        var errorResponse = notFoundResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.BOOK_NOT_FOUND);
    }

    [Fact]
    public async Task UpdateBook_Should_UpdateImageWhenBase64Provided_And_DeleteOldImage()
    {
        // Arrange
        var bookId = 1;
        var existingBook = TestDataFactory.CreateTestBook(bookId, "Book with Old Image");
        existingBook.ImageUrl = "https://example.com/old-image.jpg";

        var updatedBook = TestDataFactory.CreateTestBook(bookId, "Book with New Image");
        updatedBook.ImageUrl = "data:image/jpeg;base64,newimagedata";

        var processedBook = TestDataFactory.CreateTestBook(bookId, "Book with New Image");
        processedBook.ImageUrl = "https://example.com/new-image.jpg";

        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(existingBook);
        _mockImageRepository.Setup(i => i.SaveImageAsync("data:image/jpeg;base64,newimagedata"))
            .ReturnsAsync("https://example.com/new-image.jpg");
        _mockBookRepository.Setup(r => r.UpdateBookAsync(It.IsAny<Book>()))
            .Returns(Task.CompletedTask);
        _mockBookRepository.SetupSequence(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(existingBook)  // First call in UpdateBook method
            .ReturnsAsync(processedBook); // Second call to return updated book

        // Act
        var result = await _controller.UpdateBook(bookId, updatedBook);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockImageRepository.Verify(i => i.DeleteImage("https://example.com/old-image.jpg"), Times.Once);
        _mockImageRepository.Verify(i => i.SaveImageAsync("data:image/jpeg;base64,newimagedata"), Times.Once);
    }

    #endregion

    #region DeleteBook Tests

    [Fact]
    public async Task DeleteBook_Should_ReturnOkWithDeletedBook_When_BookExists()
    {
        // Arrange
        var bookId = 1;
        var book = TestDataFactory.CreateTestBook(bookId, "Book To Delete");
        book.ImageUrl = "https://example.com/image.jpg";

        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(book);
        _mockBookRepository.Setup(r => r.DeleteBookAsync(bookId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteBook(bookId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(book);

        _mockImageRepository.Verify(i => i.DeleteImage("https://example.com/image.jpg"), Times.Once);
        _mockBookRepository.Verify(r => r.DeleteBookAsync(bookId), Times.Once);
    }

    [Fact]
    public async Task DeleteBook_Should_ReturnNotFound_When_BookDoesNotExist()
    {
        // Arrange
        var bookId = 999;
        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _controller.DeleteBook(bookId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.StatusCode.Should().Be(404);

        var errorResponse = notFoundResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.BOOK_NOT_FOUND);
    }

    [Fact]
    public async Task DeleteBook_Should_ReturnInternalServerError_When_DeleteFails()
    {
        // Arrange
        var bookId = 1;
        var book = TestDataFactory.CreateTestBook(bookId, "Failing Delete Book");

        _mockBookRepository.Setup(r => r.GetBookByIdAsync(bookId))
            .ReturnsAsync(book);
        _mockBookRepository.Setup(r => r.DeleteBookAsync(bookId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.DeleteBook(bookId);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.BOOK_DELETE_FAILED);
    }

    #endregion

    #region SearchBooks Tests

    [Fact]
    public async Task SearchBooks_Should_ReturnOkWithResults_When_QueryProvided()
    {
        // Arrange
        var searchQuery = "programming";
        var books = TestDataFactory.CreateTestBooks(2);

        _mockBookRepository.Setup(r => r.SearchBooksAsync(searchQuery))
            .ReturnsAsync(books);

        // Act
        var result = await _controller.SearchBooks(searchQuery);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task SearchBooks_Should_UseDefaultParameters_When_InvalidParametersProvided()
    {
        // Arrange
        var books = TestDataFactory.CreateTestBooks(1);

        _mockBookRepository.Setup(r => r.SearchBooksAsync(string.Empty))
            .ReturnsAsync(books);

        // Act - Test with invalid page and pageSize
        var result = await _controller.SearchBooks(null, page: -1, pageSize: 100);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _mockBookRepository.Verify(r => r.SearchBooksAsync(string.Empty), Times.Once);
    }

    [Fact]
    public async Task SearchBooks_Should_ReturnInternalServerError_When_SearchFails()
    {
        // Arrange
        _mockBookRepository.Setup(r => r.SearchBooksAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Search error"));

        // Act
        var result = await _controller.SearchBooks("test");

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INTERNAL_SERVER_ERROR);
    }

    #endregion
}
