using Backend.Data;
using Backend.Models;
using Backend.Tests.TestHelpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.Tests.Data;

public class UserRepositoryTests : IDisposable
{
    private readonly UserContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext($"UserRepositoryTests_{Guid.NewGuid()}");
        _userRepository = new UserRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateUser_Should_AddUserToDatabase_When_ValidUserProvided()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(
            id: 0, // Will be set by EF
            email: "newuser@example.com",
            name: "New User"
        );

        // Act
        var result = await _userRepository.CreateUser(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Email.Should().Be("newuser@example.com");
        result.Name.Should().Be("New User");

        var userInDb = await _context.Users.FindAsync(result.Id);
        userInDb.Should().NotBeNull();
        userInDb!.Email.Should().Be("newuser@example.com");
    }

    [Fact]
    public async Task CreateUser_Should_SetCreatedDate_When_UserCreated()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var user = TestDataFactory.CreateTestUser(email: "timetest@example.com");

        // Act
        var result = await _userRepository.CreateUser(user);

        // Assert
        result.CreatedAt.Should().BeAfter(beforeCreation);
        result.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnUser_When_UserExists()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(email: "existing@example.com");
        await _userRepository.CreateUser(user);

        // Act
        var result = await _userRepository.GetUserByEmail("existing@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("existing@example.com");
        result.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnNull_When_UserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetUserByEmail("nonexistent@example.com");

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetUserByEmail_Should_ReturnNull_When_EmailIsInvalid(string email)
    {
        // Act
        var result = await _userRepository.GetUserByEmail(email);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnNull_When_EmailIsNull()
    {
        // Act
        var result = await _userRepository.GetUserByEmail(null!);

        // Assert - Should return null for null email, not throw exception
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByEmail_Should_BeCaseInsensitive_When_SearchingForUser()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(email: "CaseTest@Example.Com");
        await _userRepository.CreateUser(user);

        // Act
        var result1 = await _userRepository.GetUserByEmail("casetest@example.com");
        var result2 = await _userRepository.GetUserByEmail("CASETEST@EXAMPLE.COM");

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1!.Id.Should().Be(result2!.Id);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnUser_When_UserExists()
    {
        // Arrange
        var user = TestDataFactory.CreateTestUser(email: "getbyid@example.com");
        var createdUser = await _userRepository.CreateUser(user);

        // Act
        var result = await _userRepository.GetUserById(createdUser.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdUser.Id);
        result.Email.Should().Be("getbyid@example.com");
    }

    [Fact]
    public async Task GetUserById_Should_ReturnNull_When_UserDoesNotExist()
    {
        // Act
        var result = await _userRepository.GetUserById(999);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task GetUserById_Should_ReturnNull_When_InvalidIdProvided(int invalidId)
    {
        // Act
        var result = await _userRepository.GetUserById(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUser_Should_ThrowException_When_DuplicateEmailProvided()
    {
        // Arrange
        var user1 = TestDataFactory.CreateTestUser(email: "duplicate@example.com", name: "User 1");
        var user2 = TestDataFactory.CreateTestUser(email: "duplicate@example.com", name: "User 2");

        await _userRepository.CreateUser(user1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _userRepository.CreateUser(user2));
    }

    [Fact]
    public async Task CreateUser_Should_StoreProvidedPassword_When_UserCreated()
    {
        // Arrange - Repository expects password to already be hashed by controller
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("plaintextpassword");
        var user = new User
        {
            Name = "Password Test User",
            Email = "passwordtest@example.com",
            Password = hashedPassword
        };

        // Act
        var result = await _userRepository.CreateUser(user);

        // Assert
        result.Password.Should().Be(hashedPassword);
        result.Password.Should().NotBeNullOrEmpty();

        // Verify password is stored as provided (already hashed)
        var userInDb = await _context.Users.FindAsync(result.Id);
        userInDb!.Password.Should().Be(hashedPassword);
        userInDb.Password.Should().StartWith("$2"); // BCrypt hash format
    }

    [Fact]
    public async Task CreateMultipleUsers_Should_AssignUniqueIds_When_UsersCreated()
    {
        // Arrange
        var user1 = TestDataFactory.CreateTestUser(email: "user1@example.com");
        var user2 = TestDataFactory.CreateTestUser(email: "user2@example.com");
        var user3 = TestDataFactory.CreateTestUser(email: "user3@example.com");

        // Act
        var result1 = await _userRepository.CreateUser(user1);
        var result2 = await _userRepository.CreateUser(user2);
        var result3 = await _userRepository.CreateUser(user3);

        // Assert
        var ids = new[] { result1.Id, result2.Id, result3.Id };
        ids.Should().OnlyHaveUniqueItems();
        ids.Should().AllSatisfy(id => id.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnUserWithCorrectProperties_When_UserExists()
    {
        // Arrange
        var originalUser = new User
        {
            Name = "Full Property Test",
            Email = "fulltest@example.com",
            Password = "hashedpassword",
            PhoneNumber = "1234567890",
            Address = "123 Test Street",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
        await _userRepository.CreateUser(originalUser);

        // Act
        var result = await _userRepository.GetUserByEmail("fulltest@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Full Property Test");
        result.Email.Should().Be("fulltest@example.com");
        result.PhoneNumber.Should().Be("1234567890");
        result.Address.Should().Be("123 Test Street");
        result.CreatedAt.Should().BeCloseTo(originalUser.CreatedAt, TimeSpan.FromSeconds(1));
    }
}
