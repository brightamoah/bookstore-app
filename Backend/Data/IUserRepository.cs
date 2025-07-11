using Backend.Models;

namespace Backend.Data
{
    public interface IUserRepository
    {
        User CreateUser(User user);
        User? GetUserByEmail(string email);
        User? GetUserById(int id);
    }
}