using Microsoft.EntityFrameworkCore;
using ShoppingCartNewApi.Entities;

namespace ShoppingCartNewApi.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Rice",Quantity = 100},
                new Product { ProductId = 2, ProductName = "Wheat", Quantity = 100 },
                new Product { ProductId = 3, ProductName = "Sugar", Quantity = 100 },
                new Product { ProductId = 4, ProductName = "Salt", Quantity = 100 },
                new Product { ProductId = 5, ProductName = "Onion", Quantity = 100 });
        }
    }
}
