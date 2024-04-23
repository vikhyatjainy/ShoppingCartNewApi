using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.XUnit.Test.Services
{
    public class CartServiceTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly ICartService _cartService;

        public CartServiceTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new DataContext(options);
            SeedData();
            _cartService = new CartService(_context);
        }

        private void SeedData()
        {
            var userId = Guid.NewGuid();
            _context.Carts.AddRange(
                new Cart { CartId = Guid.NewGuid(), UserId = userId, ProductId = 1, Quantity = 2 },
                new Cart { CartId = Guid.NewGuid(), UserId = userId, ProductId = 2, Quantity = 3 }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task ViewCartAsync_ReturnsCartItems()
        {
            // Arrange
            var userId = _context.Carts.First().UserId;

            // Act
            var cartItems = await _cartService.ViewCartAsync(userId);

            // Assert
            Assert.NotNull(cartItems);
            Assert.Equal(2, cartItems.Count());
        }

        [Fact]
        public async Task AddToCartAsync_AddsNewCartItem()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cart = new Cart { UserId = userId, ProductId = 3, Quantity = 1 };

            // Act
            await _cartService.AddToCartAsync(cart);
            await _cartService.SaveChangesAsync();

            // Assert
            var addedCartItem = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == cart.ProductId);
            Assert.NotNull(addedCartItem);
            Assert.Equal(cart.Quantity, addedCartItem.Quantity);
        }

        [Fact]
        public async Task RemoveFromCart_RemovesCartItem()
        {
            // Arrange
            var userId = _context.Carts.First().UserId;
            var productIdToRemove = _context.Carts.First().ProductId;
            var cartItemToRemove = await _cartService.GetExistingCartItemAsync(userId, productIdToRemove);

            // Act
            await _cartService.RemoveFromCart(cartItemToRemove);
            await _cartService.SaveChangesAsync();

            // Assert
            var removedCartItem = await _context.Carts.FirstOrDefaultAsync(c => c.CartId == cartItemToRemove.CartId);
            Assert.Null(removedCartItem);
        }

        [Fact]
        public async Task IsCartQuantityValid_ValidCart_ReturnsTrue()
        {
            // Arrange
            var cart = new Cart { ProductId = 1, Quantity = 2 };

            // Act
            var isValid = await _cartService.IsCartQuantityValid(cart);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public async Task IsCartQuantityValid_InvalidCart_ReturnsFalse()
        {
            // Arrange
            var cart = new Cart { ProductId = 1, Quantity = -1 };

            // Act
            var isValid = await _cartService.IsCartQuantityValid(cart);

            // Assert
            Assert.False(isValid);
        }
    }
}
