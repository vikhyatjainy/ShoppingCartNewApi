using Microsoft.EntityFrameworkCore;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;

        public ProductService(DataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _dataContext.Products.ToListAsync();
        }
    }
}