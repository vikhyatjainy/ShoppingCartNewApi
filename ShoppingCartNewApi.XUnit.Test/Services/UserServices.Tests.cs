using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartNewApi.Controllers;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.XUnit.Test.Services
{
    public class UserControllerTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Set up an in-memory database for testing
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new DataContext(options);
            _userService =
                new UserService(_context, null); // Pass null for IConfiguration since it's not used in these tests
            _controller = new UserController(_userService);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task RegisterUser_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var userDto = new UserDto { UserName = "testuser", Password = "password123" };

            // Act
            var result = await _controller.RegisterUser(userDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User is successfully registered.", ((OkObjectResult)result).Value);
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ReturnsToken()
        {
            var userService = new UserService(MockDataContext(), MockConfiguration());
            var user = new User { UserId = Guid.NewGuid(), UserName = "testuser", Password = "password123" };

            // Act
            var token = await userService.LoginUserAsync(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token); // Ensure the token is not empty
            Assert.True(token.Length > 0); // Ensure the token has a non-zero length
        }

        // Mock DataContext for testing
        private DataContext MockDataContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new DataContext(options);
        }

        // Mock IConfiguration for testing
        private IConfiguration MockConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Authentication:SecretForKey", "YourSecretStrongAndSecureKeyIsHere" },
                    { "Authentication:Issuer", "YourIssuerHere" },
                    { "Authentication:Audience", "YourAudienceHere" }
                })
                .Build();
            return config;
        }

        [Fact]
        public async Task LoginUser_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var userDto = new UserDto { UserName = "testuser", Password = "password123" };

            // Act
            var result = await _controller.LoginUser(userDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}