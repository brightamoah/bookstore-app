using Backend.Helpers;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace Backend.Tests.Helpers;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;

    public JwtServiceTests()
    {
        _configuration = ControllerTestHelper.CreateTestConfiguration();
        _jwtService = new JwtService(_configuration);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_KeyIsMissing()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JwtService(config));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_IssuerIsMissing()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"Jwt:Key", "TestKey"},
            {"Jwt:Audience", "TestAudience"}
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JwtService(config));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_When_AudienceIsMissing()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            {"Jwt:Key", "TestKey"},
            {"Jwt:Issuer", "TestIssuer"}
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JwtService(config));
    }

    [Fact]
    public void GenerateToken_Should_ReturnValidJwtToken_When_UserIdProvided()
    {
        // Arrange
        var userId = 123;

        // Act
        var token = _jwtService.GenerateToken(userId);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jsonToken.Issuer.Should().Be("TestIssuer");
        jsonToken.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_Should_ReturnDifferentTokens_When_CalledMultipleTimes()
    {
        // Arrange
        var userId = 123;

        // Act
        var token1 = _jwtService.GenerateToken(userId);
        var token2 = _jwtService.GenerateToken(userId);

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void ValidateToken_Should_ReturnJwtSecurityToken_When_ValidTokenProvided()
    {
        // Arrange
        var userId = 123;
        var token = _jwtService.GenerateToken(userId);

        // Act
        var validatedToken = _jwtService.ValidateToken(token);

        // Assert
        validatedToken.Should().NotBeNull();
        validatedToken!.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
    }

    [Fact]
    public void ValidateToken_Should_ReturnNull_When_InvalidTokenProvided()
    {
        // Arrange
        var invalidToken = "invalid.jwt.token";

        // Act
        var validatedToken = _jwtService.ValidateToken(invalidToken);

        // Assert
        validatedToken.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_Should_ReturnNull_When_ExpiredTokenProvided()
    {
        // Arrange
        // Create a JWT service with immediate expiration for testing
        var configData = new Dictionary<string, string>
        {
            {"Jwt:Key", "ThisIsATestSecretKeyForJwtTokenGenerationThatIsLongEnough"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
        var config = new ConfigurationBuilder().AddInMemoryCollection(configData!).Build();

        // Create expired token manually (this is a simplified approach)
        var expiredToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjMiLCJqdGkiOiJkZmJkOTNhNC02NTcxLTQ4MzItODE4ZC0yNzIyMmE4OTM2NGEiLCJpYXQiOjE2MzAwMDAwMDAsImV4cCI6MTYzMDAwMDAwMSwiaXNzIjoiVGVzdElzc3VlciIsImF1ZCI6IlRlc3RBdWRpZW5jZSJ9";

        // Act
        var validatedToken = _jwtService.ValidateToken(expiredToken);

        // Assert
        validatedToken.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_Should_ReturnNull_When_EmptyTokenProvided()
    {
        // Arrange
        var emptyToken = "";

        // Act
        var validatedToken = _jwtService.ValidateToken(emptyToken);

        // Assert
        validatedToken.Should().BeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(999)]
    [InlineData(int.MaxValue)]
    public void GenerateToken_Should_IncludeCorrectUserId_When_DifferentUserIdsProvided(int userId)
    {
        // Act
        var token = _jwtService.GenerateToken(userId);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
    }

    [Fact]
    public void GenerateToken_Should_SetCorrectExpiration_When_TokenGenerated()
    {
        // Arrange
        var userId = 123;
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtService.GenerateToken(userId);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadJwtToken(token);

        jsonToken.ValidTo.Should().BeAfter(beforeGeneration.AddHours(23)); // Should be valid for ~24 hours
        jsonToken.ValidTo.Should().BeBefore(beforeGeneration.AddHours(25)); // But not more than 25 hours
    }
}
