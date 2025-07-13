using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[Route("api/books")]
[ApiController]
[Authorize]
public class BooksController(IBookRepository repository, IImageRepository imageRepository) : Controller
{
    private readonly IBookRepository _repository = repository;
    private readonly IImageRepository _imageRepository = imageRepository;

    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        try
        {
            var books = await _repository.GetAllBooksAsync();
            return Ok(books);
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Failed to retrieve books. Please try again later.",
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        try
        {
            var book = await _repository.GetBookByIdAsync(id);
            if (book == null)
                return NotFound(new ErrorResponse
                {
                    Message = "Book not found. Please check the book ID and try again.",
                    ErrorCode = ErrorCodes.BOOK_NOT_FOUND,
                    TraceId = HttpContext.TraceIdentifier
                });

            return Ok(book);
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Failed to retrieve book information",
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] Book book)
    {
        if (!ModelState.IsValid)
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                );

            return BadRequest(new ValidationErrorResponse
            {
                Message = "Book data validation failed",
                ErrorCode = ErrorCodes.VALIDATION_FAILED,
                Errors = validationErrors,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            if (!string.IsNullOrEmpty(book.ImageUrl) && book.ImageUrl.StartsWith("data:image"))
            {
                book.ImageUrl = await _imageRepository.SaveImageAsync(book.ImageUrl);
            }

            var createdBook = await _repository.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.BookId }, createdBook);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("image"))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Image processing failed. Please check your image format and try again.",
                ErrorCode = ErrorCodes.IMAGE_PROCESSING_FAILED,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (ArgumentException)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Invalid book data provided",
                ErrorCode = ErrorCodes.INVALID_BOOK_DATA,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Book creation failed due to a server error",
                ErrorCode = ErrorCodes.BOOK_CREATION_FAILED,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
    {
        if (id != book.BookId)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Book ID mismatch. Please ensure the ID in the URL matches the book data.",
                ErrorCode = ErrorCodes.INVALID_BOOK_DATA,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        if (!ModelState.IsValid || string.IsNullOrEmpty(book.Author))
        {
            var validationErrors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                );

            if (string.IsNullOrEmpty(book.Author))
            {
                validationErrors["Author"] = ["Author is required"];
            }

            return BadRequest(new ValidationErrorResponse
            {
                Message = "Book data validation failed",
                ErrorCode = ErrorCodes.VALIDATION_FAILED,
                Errors = validationErrors,
                TraceId = HttpContext.TraceIdentifier
            });
        }

        try
        {
            var existingBook = await _repository.GetBookByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Book not found. Please check the book ID and try again.",
                    ErrorCode = ErrorCodes.BOOK_NOT_FOUND,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            book.CreatedAt = existingBook.CreatedAt;
            book.UpdatedAt = DateTime.UtcNow;


            if (!string.IsNullOrEmpty(book.ImageUrl) && book.ImageUrl.StartsWith("data:image"))
            {
                if (!string.IsNullOrEmpty(existingBook.ImageUrl))
                {
                    _imageRepository.DeleteImage(existingBook.ImageUrl);
                }

                book.ImageUrl = await _imageRepository.SaveImageAsync(book.ImageUrl);
            }
            else if (string.IsNullOrEmpty(book.ImageUrl))
            {
                book.ImageUrl = existingBook.ImageUrl;
            }

            await _repository.UpdateBookAsync(book);

            var updatedBook = await _repository.GetBookByIdAsync(id);
            return Ok(updatedBook);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("image"))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Image processing failed. Please check your image format and try again.",
                ErrorCode = ErrorCodes.IMAGE_PROCESSING_FAILED,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Book not found. Please check the book ID and try again.",
                ErrorCode = ErrorCodes.BOOK_NOT_FOUND,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Book update failed due to a server error",
                ErrorCode = ErrorCodes.BOOK_UPDATE_FAILED,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var book = await _repository.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound(new ErrorResponse
                {
                    Message = "Book not found. Please check the book ID and try again.",
                    ErrorCode = ErrorCodes.BOOK_NOT_FOUND,
                    TraceId = HttpContext.TraceIdentifier
                });
            }


            if (!string.IsNullOrEmpty(book.ImageUrl))
            {
                _imageRepository.DeleteImage(book.ImageUrl);
            }
            await _repository.DeleteBookAsync(id);
            return Ok(book);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("image"))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Image deletion failed during book removal",
                ErrorCode = ErrorCodes.IMAGE_PROCESSING_FAILED,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Book deletion failed due to a server error",
                ErrorCode = ErrorCodes.BOOK_DELETE_FAILED,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchBooks(
            [FromQuery] string? query = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
    {

        try
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var books = await _repository.SearchBooksAsync(query ?? string.Empty);

            return Ok(new
            {
                books,
                query,
                page,
                pageSize,
                totalResults = books.Count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Error searching books",
                ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                Details = ex.Message,
                TraceId = HttpContext.TraceIdentifier
            });
        }
    }
}