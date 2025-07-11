using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;


public class BookRepository(UserContext context) : IBookRepository
{
    private readonly UserContext _context = context;

    public async Task<Book> CreateBookAsync(Book book)
    {
        book.CreatedAt = DateTime.UtcNow;
        book.UpdatedAt = null;
        _context.Books.Add(book);
        book.BookId = await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books
           .OrderByDescending(b => b.CreatedAt)
           .ToListAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        book.UpdatedAt = DateTime.UtcNow;
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        return await SearchBooksAsync(searchTerm, 1, 10);
    }

    public async Task<List<Book>> SearchBooksAsync(string searchTerm, int page = 1, int pageSize = 10)
    {
        var query = _context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower().Trim();

            query = query.Where(b =>
                b.BookName.ToLower().Contains(searchTerm) ||
                b.Author.ToLower().Contains(searchTerm) ||
                b.Category.ToLower().Contains(searchTerm) ||
                b.Description.ToLower().Contains(searchTerm)
            );
        }

        return await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

    }
}

