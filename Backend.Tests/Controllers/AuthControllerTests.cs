using Backend.Controllers;
using Backend.Data;
using Backend.DataTransferObjects;
using Backend.Helpers;
using Backend.Models;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Backend.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IWebHostEnvironment> _mockEnvironment;
    private readonly AuthController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<HttpResponse> _mockResponse;
    private readonly Mock<IResponseCookies> _mockCookies;

    public AuthControllerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockEnvironment = new Mock<IWebHostEnvironment>();

        // Setup environment to return false for IsProduction (test environment)
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns(Environments.Development);

        _controller = new AuthController(_mockUserRepository.Object, _mockJwtService.Object, _mockEnvironment.Object);

        // Setup HttpContext mocking for cookie operations
        _mockHttpContext = new Mock<HttpContext>();
        _mockResponse = new Mock<HttpResponse>();
        _mockCookies = new Mock<IResponseCookies>();

        _mockResponse.Setup(r => r.Cookies).Returns(_mockCookies.Object);
        _mockHttpContext.Setup(c => c.Response).Returns(_mockResponse.Object);
        _mockHttpContext.Setup(c => c.TraceIdentifier).Returns("test-trace-id");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _mockHttpContext.Object
        };
    }

    public void Dispose()
    {
        _controller.Dispose();
    }

    #region Signup Tests

    [Fact]
    public async Task Signup_Should_ReturnCreatedResult_When_ValidDataProvided()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        var createdUser = TestDataFactory.CreateTestUser(1, "john@example.com", "John Doe");

        _mockUserRepository.Setup(r => r.GetUserByEmail(signupDto.Email))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _controller.Signup(signupDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);

        createdResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Signup_Should_ReturnConflict_When_UserAlreadyExists()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Name = "John Doe",
            Email = "existing@example.com",
            Password = "password123"
        };

        var existingUser = TestDataFactory.CreateTestUser(1, "existing@example.com");

        _mockUserRepository.Setup(r => r.GetUserByEmail(signupDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _controller.Signup(signupDto);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
        var conflictResult = result as ConflictObjectResult;
        conflictResult!.StatusCode.Should().Be(409);

        var errorResponse = conflictResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.USER_ALREADY_EXISTS);
    }

    [Fact]
    public async Task Signup_Should_ReturnBadRequest_When_PasswordTooShort()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "short" // Less than 8 characters
        };

        _mockUserRepository.Setup(r => r.GetUserByEmail(signupDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Signup(signupDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be(400);

        var errorResponse = badRequestResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.WEAK_PASSWORD);
        errorResponse.Message.Should().Contain("at least 8 characters");
    }

    [Fact]
    public async Task Signup_Should_HashPassword_When_CreatingUser()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        User capturedUser = null!;
        _mockUserRepository.Setup(r => r.GetUserByEmail(signupDto.Email))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>()))
            .Callback<User>(user => capturedUser = user)
            .ReturnsAsync(TestDataFactory.CreateTestUser());

        // Act
        await _controller.Signup(signupDto);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser.Password.Should().NotBe("password123");
        capturedUser.Password.Should().StartWith("$2"); // BCrypt hash format
        BCrypt.Net.BCrypt.Verify("password123", capturedUser.Password).Should().BeTrue();
    }

    [Fact]
    public async Task Signup_Should_ReturnInternalServerError_When_CreateUserThrowsException()
    {
        // Arrange
        var signupDto = new SignupDto
        {
            Name = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.GetUserByEmail(signupDto.Email))
            .ReturnsAsync((User?)null);
        _mockUserRepository.Setup(r => r.CreateUser(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Signup(signupDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INTERNAL_SERVER_ERROR);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_Should_ReturnOkWithJwtCookie_When_ValidCredentialsProvided()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john@example.com",
            Password = "password123"
        };

        var user = TestDataFactory.CreateTestUser(1, "john@example.com", "John Doe");
        user.Password = BCrypt.Net.BCrypt.HashPassword("password123");

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginDto.Email))
            .ReturnsAsync(user);
        _mockJwtService.Setup(j => j.GenerateToken(user.Id))
            .Returns("fake-jwt-token");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        // Verify JWT cookie was set
        _mockCookies.Verify(c => c.Append(
            "jwtToken",
            "fake-jwt-token",
            It.IsAny<CookieOptions>()), Times.Once);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_UserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.StatusCode.Should().Be(401);

        var errorResponse = unauthorizedResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INVALID_CREDENTIALS);
    }

    [Fact]
    public async Task Login_Should_ReturnUnauthorized_When_PasswordIncorrect()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john@example.com",
            Password = "wrongpassword"
        };

        var user = TestDataFactory.CreateTestUser(1, "john@example.com", "John Doe");
        user.Password = BCrypt.Net.BCrypt.HashPassword("correctpassword");

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginDto.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.StatusCode.Should().Be(401);

        var errorResponse = unauthorizedResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INVALID_CREDENTIALS);
    }

    [Fact]
    public async Task Login_Should_SetSecureCookieOptions_When_SuccessfulLogin()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john@example.com",
            Password = "password123"
        };

        var user = TestDataFactory.CreateTestUser(1, "john@example.com");
        user.Password = BCrypt.Net.BCrypt.HashPassword("password123");

        _mockUserRepository.Setup(r => r.GetUserByEmail(loginDto.Email))
            .ReturnsAsync(user);
        _mockJwtService.Setup(j => j.GenerateToken(user.Id))
            .Returns("fake-jwt-token");

        // Act
        await _controller.Login(loginDto);

        // Assert - In test environment, Secure should be false
        _mockCookies.Verify(c => c.Append(
            "jwtToken",
            "fake-jwt-token",
            It.Is<CookieOptions>(options =>
                options.HttpOnly == true &&
                options.Secure == false && // Changed to false for test environment
                options.SameSite == SameSiteMode.Strict &&
                options.Expires.HasValue)), Times.Once);
    }

    #endregion

    #region GetCurrentUser Tests

    [Fact]
    public async Task GetCurrentUser_Should_ReturnUnauthorized_When_NoCookieProvided()
    {
        // Arrange
        var mockRequest = new Mock<HttpRequest>();
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c["jwtToken"]).Returns((string?)null);
        mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
        _mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult!.StatusCode.Should().Be(401);

        var errorResponse = unauthorizedResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.TOKEN_EXPIRED);
    }

    [Fact]
    public async Task GetCurrentUser_Should_ReturnUnauthorized_When_TokenInvalid()
    {
        // Arrange
        var mockRequest = new Mock<HttpRequest>();
        var mockCookies = new Mock<IRequestCookieCollection>();
        mockCookies.Setup(c => c["jwtToken"]).Returns("invalid-token");
        mockRequest.Setup(r => r.Cookies).Returns(mockCookies.Object);
        _mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

        _mockJwtService.Setup(j => j.ValidateToken("invalid-token"))
            .Returns((System.IdentityModel.Tokens.Jwt.JwtSecurityToken?)null);

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion

    #region Logout Tests

    [Fact]
    public void Logout_Should_ReturnOkAndDeleteCookie_When_Called()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);

        // Verify cookie was deleted
        _mockCookies.Verify(c => c.Delete(
            "jwtToken",
            It.IsAny<CookieOptions>()), Times.Once);
    }

    [Fact]
    public void Logout_Should_ReturnInternalServerError_When_ExceptionThrown()
    {
        // Arrange
        _mockCookies.Setup(c => c.Delete(It.IsAny<string>(), It.IsAny<CookieOptions>()))
            .Throws(new Exception("Cookie error"));

        // Act
        var result = _controller.Logout();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);

        var errorResponse = objectResult.Value as ErrorResponse;
        errorResponse!.ErrorCode.Should().Be(ErrorCodes.INTERNAL_SERVER_ERROR);
    }

    #endregion
}
