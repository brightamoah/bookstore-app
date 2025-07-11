using Backend.Data;
using Backend.DataTransferObjects;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Backend.Controllers
{

    [Route("api")]
    [ApiController]
    public class AuthController(IUserRepository repository, JwtService jwtService) : Controller
    {
        private readonly IUserRepository _repository = repository;
        private readonly JwtService _jwtService = jwtService;

        [HttpPost("signup")]
        public IActionResult Signup(SignupDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

            };

            _repository.CreateUser(user);

            return Ok(new { message = "User created successfully" });

        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _repository.GetUserByEmail(dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid email or password" });
            }

            var jwtToken = _jwtService.GenerateToken(user.Id);

            Response.Cookies.Append("jwtToken", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddHours(1)

            });

            return Ok(new
            {
                message = "Login successful"
            });

        }
        [HttpGet("user")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var jwt = Request.Cookies["jwtToken"];

                if (string.IsNullOrEmpty(jwt))
                {
                    return Unauthorized(new { message = "No token provided" });
                }

                var token = _jwtService.ValidateToken(jwt);

                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token payload" });
                }

                var user = _repository.GetUserById(userId);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwtToken");

            return Ok(new { message = "Logout successful" });
        }
    }


}