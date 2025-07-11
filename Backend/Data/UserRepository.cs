using Backend.Models;

namespace Backend.Data
{
    public class UserRepository(UserContext context) : IUserRepository
    {
        private readonly UserContext _context = context;

        public Task<User> CreateUser(User user)
        {

            _context.Users.Add(user);
            user.Id = _context.SaveChanges();
            return Task.FromResult(user);
        }

        public Task<User?> GetUserByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult<User?>(user);
        }

        public Task<User?> GetUserById(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }
    }
}