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
        public async Task<IActionResult> Signup(SignupDto dto)
        {

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                    );

                return BadRequest(new ValidationErrorResponse
                {
                    Message = "Registration data validation failed",
                    ErrorCode = ErrorCodes.VALIDATION_FAILED,
                    Errors = validationErrors,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            try
            {
                var existingUser = await _repository.GetUserByEmail(dto.Email);
                if (existingUser != null)
                {
                    return Conflict(new ErrorResponse
                    {
                        Message = "An account with this email address already exists",
                        ErrorCode = ErrorCodes.USER_ALREADY_EXISTS,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                if (dto.Password.Length < 8)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Password must be at least 8 characters long",
                        ErrorCode = ErrorCodes.WEAK_PASSWORD,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                var user = await _repository.CreateUser(new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                });

                return CreatedAtAction(nameof(GetCurrentUser), null, new
                {
                    message = "Account created successfully. Please log in to continue.",
                    userId = user.Id
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("duplicate"))
            {
                return Conflict(new ErrorResponse
                {
                    Message = "An account with this email address already exists",
                    ErrorCode = ErrorCodes.USER_ALREADY_EXISTS,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Account creation failed due to a server error",
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                    );

                return BadRequest(new ValidationErrorResponse
                {
                    Message = "Login data validation failed",
                    ErrorCode = ErrorCodes.VALIDATION_FAILED,
                    Errors = validationErrors,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            try
            {
                var user = await _repository.GetUserByEmail(dto.Email);
                if (user == null)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid email or password. Please check your credentials and try again.",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid email or password. Please check your credentials and try again.",
                        ErrorCode = ErrorCodes.INVALID_CREDENTIALS,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                var jwt = _jwtService.GenerateToken(user.Id);

                Response.Cookies.Append("jwtToken", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                return Ok(new
                {
                    message = "Login successful",
                    user = new { user.Id, user.Name, user.Email }
                });

            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Login failed due to a server error. Please try again later.",
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    TraceId = HttpContext.TraceIdentifier
                });
            }

        }



        [HttpGet("user")]
        public async Task<IActionResult> GetCurrentUser()
        {

            try
            {
                var jwt = Request.Cookies["jwtToken"];

                if (string.IsNullOrEmpty(jwt))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid authentication token. Please log in again.",
                        ErrorCode = ErrorCodes.TOKEN_EXPIRED,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                var token = _jwtService.ValidateToken(jwt);

                if (token == null)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid authentication token. Please log in again.",
                        ErrorCode = ErrorCodes.TOKEN_EXPIRED,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }



                if (!int.TryParse(token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value, out int userId))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid authentication token. Please log in again.",
                        ErrorCode = ErrorCodes.TOKEN_EXPIRED,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                if (userId == 0)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Invalid authentication token. Please log in again.",
                        ErrorCode = ErrorCodes.TOKEN_EXPIRED,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                var user = await _repository.GetUserById(userId);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "User account not found. Please contact support if this issue persists.",
                        ErrorCode = ErrorCodes.USER_NOT_FOUND,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

                return Ok(new { user.Id, user.Name, user.Email });
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Failed to retrieve user information",
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("jwtToken", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception)
            {
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Logout failed. Please clear your browser cookies manually.",
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }
    }


}