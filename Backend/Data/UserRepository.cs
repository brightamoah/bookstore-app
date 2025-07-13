using Backend.Models;
using BCrypt.Net;

namespace Backend.Data
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        private readonly UserContext _context = context;

        public Task<User> CreateUser(User user)
        {
            // Check for duplicate email
            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists.");
            }

            // Password should already be hashed by the caller
            _context.Users.Add(user);
            _context.SaveChanges();
            return Task.FromResult(user);
        }

        public Task<User?> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return Task.FromResult<User?>(null);

            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            return Task.FromResult<User?>(user);
        }

        public Task<User?> GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
    }
}