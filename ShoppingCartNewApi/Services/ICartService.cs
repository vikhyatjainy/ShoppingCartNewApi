using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Services
{
    public interface ICartService
    {
        // View cart
        Task<bool> GetUserAsync(Guid userId);
        Task<IEnumerable<Cart>> ViewCartAsync(Guid userId);

        // Add to cart
        Task<bool> IsCartQuantityValid(Cart cart);
        Task<Cart?> GetProductsExistingAsync(Guid userId, Cart cart);
        Task AddToCartAsync(Cart cart);
        Task SaveChangesAsync();

        // Delete from cart
        Task<Cart?> GetExistingCartItemAsync(Guid userId, int productId);
        Task RemoveFromCart(Cart cartItem);

        // Checkout cart
        //Task<bool> GetProductAsync(Guid userId, CartDto cartDto);
        Task<Product?> GetProductsAsync(Guid userId, Cart cart);
        Task<IEnumerable<Cart>> GetCartItems(Guid userId);
        Task<Product?> GetProductFromCart(Cart cart);
        Task UpdateProductsAfterCheckout(Cart cartItem);
    }
}

