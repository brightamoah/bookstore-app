using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;


public class BookRepository(UserContext context) : IBookRepository
{
    private readonly UserContext _context = context;

    public async Task<Book> CreateBookAsync(Book book)
    {
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
        return await _context.Books.ToListAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
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
}

