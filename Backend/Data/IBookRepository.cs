using Backend.Models;

namespace Backend.Data;

public interface IBookRepository
{
    Task<Book> CreateBookAsync(Book book);
    Task<Book?> GetBookByIdAsync(int id);
    Task<List<Book>> GetAllBooksAsync();
    Task UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);

}