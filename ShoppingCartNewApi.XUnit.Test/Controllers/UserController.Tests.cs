using Moq;
using ShoppingCartNewApi.Services;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartNewApi.Controllers;

namespace ShoppingCartNewApi.XUnit.Test.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _userController = new UserController(_mockUserService.Object);
    }
    [Fact]
    public async Task RegisterUser_UserAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var userDto = new UserDto { UserName = "existingUser", Password = "password" };
        var existingUser = new User { UserName = "existingUser", Password = "password", UserId = Guid.NewGuid() };

        _mockUserService.Setup(x => x.GetExistingUser(userDto)).ReturnsAsync(existingUser);
        // Act
        var result = await _userController.RegisterUser(userDto);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task RegisterUser_NewUserRegistered_ReturnsOk()
    {
        // Arrange
        var userDto = new UserDto { UserName = "existingUser", Password = "password" };
        _mockUserService.Setup(x => x.GetExistingUser(userDto)).ReturnsAsync(new User());
        // Act
        var result = await _userController.RegisterUser(userDto);
        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginUser_UserExists_ReturnsOk()
    {
        // Arrange
        var userDto = new UserDto { UserName = "testUser", Password = "testPassword" };
        var existingUser = new User { UserName = "testUser", Password = "testPassword", UserId = Guid.NewGuid() };
        var token = "testToken";
        _mockUserService.Setup(x => x.GetExistingUser(userDto)).ReturnsAsync(existingUser);
        _mockUserService.Setup(x => x.LoginUserAsync(existingUser)).ReturnsAsync(token);

        // Act
        var result = await _userController.LoginUser(userDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task LoginUser_UserNotExists_ReturnsBad()
    {
        // Arrange
        var userDto = new UserDto { UserName = "testUser", Password = "testPassword" };
        _mockUserService.Setup(x => x.GetExistingUser(userDto)).ReturnsAsync(new User());

        // Act
        var result = await _userController.LoginUser(userDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}