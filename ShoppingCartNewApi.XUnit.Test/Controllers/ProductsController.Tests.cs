using Microsoft.AspNetCore.Mvc;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Services;
using ShoppingCartNewApi.Controllers;
using Moq;

namespace ShoppingCartNewApi.XUnit.Test.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _controller = new ProductController(_productServiceMock.Object);
    }
    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithProducts()
    {
        // Arrange
        var expectedProducts = new List<Product> { new Product { ProductId = 1, Quantity = 100 }, new Product { ProductId = 2, Quantity = 100 } };

        _productServiceMock.Setup(x => x.GetProductsAsync()).ReturnsAsync(expectedProducts);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var okResult = (OkObjectResult)result.Result;
        Assert.Equal(okResult.Value, expectedProducts);
    }
}