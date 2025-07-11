using Backend.Models;

namespace Backend.Data
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        private readonly UserContext _context = context;

        public User CreateUser(User user)
        {

            _context.Users.Add(user);
            user.Id = _context.SaveChanges();
            return user;
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public User? GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}