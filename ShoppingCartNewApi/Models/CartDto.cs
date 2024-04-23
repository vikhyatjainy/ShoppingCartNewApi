using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNewApi.Models
{
    public class CartDto
    {
        [Required(ErrorMessage = "Please enter a Product Id.")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please enter a Quantity.")]
        public int Quantity { get; set; }
    }
}