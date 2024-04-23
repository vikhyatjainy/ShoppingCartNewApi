using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.XUnit.Test.Services
{
    public class ProductServiceTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly IProductService _productService;

        public ProductServiceTests()
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
            _productService = new ProductService(_context);
        }

        private void SeedData()
        {
            _context.Products.AddRange(
                new Product { ProductId = 1, ProductName = "Test Product 1", Quantity = 10 },
                new Product { ProductId = 2, ProductName = "Test Product 2", Quantity = 5 }
            );
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Act
            var products = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Equal(2, products.Count());
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsCorrectProduct()
        {
            // Arrange
            const int productId = 1;

            // Act
            var products = await _productService.GetProductsAsync();
            var product = products.FirstOrDefault(p => p.ProductId == productId);

            // Assert
            Assert.NotNull(product);
            Assert.Equal(productId, product.ProductId);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsEmptyListWhenNoProducts()
        {
            // Arrange
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();
            // Act
            var products = await _productService.GetProductsAsync();

            // Assert
            Assert.NotNull(products);
            Assert.Empty(products);
        }
    }
}
