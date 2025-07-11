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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while retrieving books", error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _repository.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(new { message = "Book not found" });
        return Ok(book);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] Book book)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            if (!string.IsNullOrEmpty(book.ImageUrl) && book.ImageUrl.StartsWith("data:image"))
            {
                book.ImageUrl = await _imageRepository.SaveImageAsync(book.ImageUrl);
            }

            var createdBook = await _repository.CreateBookAsync(book);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.BookId }, createdBook);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating book", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book book)
    {
        if (id != book.BookId)
            return BadRequest(new { message = "Book ID mismatch" });

        try
        {
            var existingBook = await _repository.GetBookByIdAsync(id);
            if (existingBook == null)
                return NotFound(new { message = "Book not found" });


            if (!string.IsNullOrEmpty(book.ImageUrl) && book.ImageUrl.StartsWith("data:image"))
            {
                if (!string.IsNullOrEmpty(existingBook.ImageUrl))
                {
                    _imageRepository.DeleteImage(existingBook.ImageUrl);
                }

                book.ImageUrl = await _imageRepository.SaveImageAsync(book.ImageUrl);
            }

            await _repository.UpdateBookAsync(book);

            var updatedBook = await _repository.GetBookByIdAsync(id);
            return Ok(updatedBook);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating book", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _repository.GetBookByIdAsync(id);
        if (book == null)
            return NotFound(new { message = "Book not found" });

        try
        {
            if (!string.IsNullOrEmpty(book.ImageUrl))
            {
                _imageRepository.DeleteImage(book.ImageUrl);
            }
            await _repository.DeleteBookAsync(id);
            return Ok(book);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting book", error = ex.Message });
        }
    }
}