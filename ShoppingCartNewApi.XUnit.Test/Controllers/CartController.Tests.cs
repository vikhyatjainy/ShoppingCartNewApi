using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingCartNewApi.Controllers;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.XUnit.Test.Controllers;

public class CartControllerTests
{
    private Mock<ICartService> _cartService;
    private CartController _cartController;

    public CartControllerTests()
    {
        _cartService = new Mock<ICartService>();
        _cartController = new CartController(_cartService.Object);
    }
    [Fact]
    public async Task ViewCartAsync_Test()
    {
        var userId = new Guid();
        var cartItems = new List<Cart>
            { new() { ProductId = 1, Quantity = 100 }, new() { ProductId = 1, Quantity = 100 } };

        _cartService.Setup(x => x.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(x => x.ViewCartAsync(userId)).ReturnsAsync(cartItems);

        var result = await _cartController.ViewCart(userId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddToCart_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 1 };
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as NotFoundObjectResult;

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(result.Value, "User does not exist.");
    }

    [Fact]
    public async Task AddToCart_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 1 };
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsAsync(userId, It.IsAny<Cart>())).ReturnsAsync(null as Product);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "Product does not exist.");
    }

    [Fact]
    public async Task AddToCart_InvalidQuantity_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 0 };
        var product = new Product();
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsAsync(userId, It.IsAny<Cart>())).ReturnsAsync(product);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "Please enter a valid quantity.");
    }

    [Fact]
    public async Task AddToCart_WhenProductExistsAndQuantityExceedsLimit_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 101 }; 
        var existingProduct = new Cart { ProductId = 1, Quantity = 90 };
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsAsync(userId, It.IsAny<Cart>()))
            .ReturnsAsync(new Product());
        _cartService.Setup(s => s.IsCartQuantityValid(It.IsAny<Cart>())).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsExistingAsync(userId, It.IsAny<Cart>())).ReturnsAsync(existingProduct);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "Shop limit exceeded");
    }

    [Fact]
    public async Task AddToCart_ItemRequestedExceededLimit_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 101 }; 
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsAsync(userId, It.IsAny<Cart>()))
            .ReturnsAsync(new Product()); 
        _cartService.Setup(s => s.IsCartQuantityValid(It.IsAny<Cart>())).ReturnsAsync(true); 
        _cartService.Setup(s => s.GetProductsExistingAsync(userId, It.IsAny<Cart>())).ReturnsAsync(null as Cart);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "Item requested exceeded its limit.");
    }

    [Fact]
    public async Task AddToCart_WhenValidProductAndQuantity_AddsToCartAndReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartDto = new CartDto { ProductId = 1, Quantity = 1 };
        _cartService.Setup(s => s.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsAsync(userId, It.IsAny<Cart>())).ReturnsAsync(new Product());
        _cartService.Setup(s => s.IsCartQuantityValid(It.IsAny<Cart>())).ReturnsAsync(true);
        _cartService.Setup(s => s.GetProductsExistingAsync(userId, It.IsAny<Cart>())).ReturnsAsync(null as Cart);

        // Act
        var result = await _cartController.AddToCart(userId, cartDto) as OkObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(result.Value, "The selected product was added to cart.");
    }

    [Fact]
    public async Task DeleteCart_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 123;
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _cartController.DeleteCart(userId, productId) as NotFoundObjectResult;

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(result.Value, "User does not exist.");
    }

    [Fact]
    public async Task DeleteCart_ProductNotExistsInDatabase_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 123;
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(service => service.GetExistingCartItemAsync(userId, productId)).ReturnsAsync(null as Cart);

        // Act
        var result = await _cartController.DeleteCart(userId, productId) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "The product does not exists in the cart.");
    }

    [Fact]
    public async Task DeleteCart_SuccessfulDeletion_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = 123;
        var cart = new Cart { UserId = userId, ProductId = productId };
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(service => service.GetExistingCartItemAsync(userId, productId)).ReturnsAsync(cart);

        // Act
        var result = await _cartController.DeleteCart(userId, productId) as OkObjectResult;

        // Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(result.Value, "Product was removed form the cart.");
    }

    [Fact]
    public async Task Checkout_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _cartController.Checkout(userId) as NotFoundObjectResult;

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(result.Value, "User does not exist.");
    }

    [Fact]
    public async Task Checkout_CartIsEmpty_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(service => service.GetCartItems(userId)).ReturnsAsync(new List<Cart>());

        // Act
        var result = await _cartController.Checkout(userId) as BadRequestObjectResult;

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(result.Value, "No item exists in the cart.");
    }

    [Fact]
    public async Task Checkout_SuccessfulCheckout_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cartItems = new List<Cart>
        {
            new Cart { UserId = userId, ProductId = 1, Quantity = 2 },
            new Cart { UserId = userId, ProductId = 2, Quantity = 1 }
        };
        _cartService.Setup(service => service.GetUserAsync(userId)).ReturnsAsync(true);
        _cartService.Setup(service => service.GetCartItems(userId)).ReturnsAsync(cartItems);

        // Act
        var result = await _cartController.Checkout(userId) as OkObjectResult;

        // 
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(result.Value, "The checkout was successful.");
    }
}