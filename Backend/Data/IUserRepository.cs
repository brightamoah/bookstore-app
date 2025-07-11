using Backend.Models;

namespace Backend.Data
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserById(int id);
    }
}