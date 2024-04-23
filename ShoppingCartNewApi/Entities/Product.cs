using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNewApi.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
