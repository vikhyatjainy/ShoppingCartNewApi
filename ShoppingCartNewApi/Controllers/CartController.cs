using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using ShoppingCartNewApi.Services;
using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Controllers
{
    [Authorize]
    [Route("shopping/cart")]
    [ApiController]

    public class CartController : ControllerBase
    {
        private readonly ICartService _service;
        public CartController(ICartService service)
        {
            _service = service;

        }
        [HttpGet("{userId}", Name = "GetCartItems")]
        public async Task<ActionResult> ViewCart(Guid userId)
        {
            if (!await _service.GetUserAsync(userId))
            {
                return NotFound("User does not exist.");
            }
            var cartItems = await _service.ViewCartAsync(userId);
            var newCarts = cartItems.Select(c => new CartDto { ProductId = c.ProductId, Quantity = c.Quantity });
            return Ok(newCarts);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> AddToCart(Guid userId, [FromBody] CartDto cartDto)
        {
            var userExists = await _service.GetUserAsync(userId);
            if (!userExists)
                return NotFound("User does not exist.");

            // Check if the product exists
            //var productExists = await _service.GetProductAsync(userId, cartDto);
            //if (!productExists)
            //    return NotFound("Product does not exist.");

            // Get the product details
            var product = await _service.GetProductsAsync(userId, new Cart { ProductId = cartDto.ProductId });
            if (product == null)
                return BadRequest("Product does not exist.");

            // Check if the quantity is valid
            if (!await _service.IsCartQuantityValid(new Cart { Quantity = cartDto.Quantity }))
                return BadRequest("Please enter a valid quantity.");

            // Check if the shop limit is exceeded
            var existingProduct = await _service.GetProductsExistingAsync(userId, new Cart { ProductId = cartDto.ProductId });
            if (existingProduct != null)
            {
                int newQuantity = existingProduct.Quantity + cartDto.Quantity;
                if (newQuantity > 100)
                    return BadRequest("Shop limit exceeded");

                existingProduct.Quantity = newQuantity;
            }
            else
            {
                // Check if the requested item quantity exceeds its limit
                if (cartDto.Quantity > 100)
                    return BadRequest("Item requested exceeded its limit.");

                // Add the product to the cart
                await _service.AddToCartAsync(new Cart
                { ProductId = cartDto.ProductId, Quantity = cartDto.Quantity, UserId = userId });
            }

            await _service.SaveChangesAsync();

            return Ok("The selected product was added to cart.");
        }

        [HttpDelete("{userId}/{productId}")]
        public async Task<ActionResult> DeleteCart(Guid userId, int productId)
        {
            if (!await _service.GetUserAsync(userId))
            {
                return NotFound("User does not exist.");
            }

            var existingCart = await _service.GetExistingCartItemAsync(userId, productId);
            if (existingCart == null)
            {
                return BadRequest("The product does not exists in the cart.");
            }

            await _service.RemoveFromCart(existingCart);
            await _service.SaveChangesAsync();

            return Ok("Product was removed form the cart.");
        }

        [HttpPost("checkout/{userId}")]
        public async Task<ActionResult> Checkout(Guid userId)
        {
            if (!await _service.GetUserAsync(userId))
            {
                return NotFound("User does not exist.");
            }

            var cartItems = await _service.GetCartItems(userId);
            if (cartItems.Count() == 0)
            {
                return BadRequest("No item exists in the cart.");
            }

            foreach (var cartItem in cartItems)
            {
                var product = await _service.GetProductFromCart(cartItem);
                if (product != null)
                {
                    product.Quantity -= cartItem.Quantity;
                    await _service.UpdateProductsAfterCheckout(cartItem);
                }
            }

            await _service.SaveChangesAsync();
            return Ok("The checkout was successful.");
        }

    }
}

