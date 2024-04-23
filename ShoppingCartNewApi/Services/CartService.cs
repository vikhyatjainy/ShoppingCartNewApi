using Microsoft.EntityFrameworkCore;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Services
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;

        public CartService(DataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        // View cart
        public async Task<bool> GetUserAsync(Guid userId)
        {
            return await _dataContext.Users.AnyAsync(u => u.UserId == userId);
        }
        public async Task<IEnumerable<Cart>> ViewCartAsync(Guid userId)
        {
            return await _dataContext.Carts.Where(c => c.UserId == userId).ToListAsync();
        }

        // Add to cart
        //public async Task<bool> GetProductAsync(Guid userId, CartDto cartDto)
        //{
        //    return await _dataContext.Products.AnyAsync(p => p.ProductId == cartDto.ProductId );
        //}
        public async Task<Product?> GetProductsAsync(Guid userId, Cart cart)
        {
            var products = await _dataContext.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId);
            if (products == null)
            {
                return null;
            }
            return products;
        }
        public async Task<bool> IsCartQuantityValid(Cart cart)
        {
            return cart.Quantity > 0;
        }
        public async Task<Cart> GetProductsExistingAsync(Guid userId, Cart cart)
        {
            return await _dataContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == cart.ProductId);
        }
        public async Task AddToCartAsync(Cart cart)
        {
            _dataContext.Carts.Add(cart);
        }
        public async Task SaveChangesAsync()
        {
            await _dataContext.SaveChangesAsync();
        }


        // Delete from cart
        public async Task<Cart?> GetExistingCartItemAsync(Guid userId, int productId)
        {
            return await _dataContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }
        public async Task RemoveFromCart(Cart cartItem)
        {
            _dataContext.Carts.Remove(cartItem);
        }

        // Checkout cart
        public async Task<IEnumerable<Cart>> GetCartItems(Guid userId)
        {
            var carts = await _dataContext.Carts.Where(c => c.UserId == userId).ToListAsync();
            return carts;
        }
        public async Task<Product?> GetProductFromCart(Cart cart)
        {
            var product = await _dataContext.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId);
            return product;
        }
        public async Task UpdateProductsAfterCheckout(Cart cartItem)
        {
            _dataContext.Carts.Remove(cartItem);
        }



    }
}