using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
    }
}