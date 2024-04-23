using Microsoft.AspNetCore.Mvc;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.Controllers
{
    [Route("shopping")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }
    }
}