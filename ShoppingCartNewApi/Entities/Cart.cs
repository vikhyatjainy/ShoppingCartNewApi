using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNewApi.Entities
{
    public class Cart
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid CartId { get; set; }
        [Required(ErrorMessage = "Please enter a Product Id.")]
        // Navigation Property (Relationship will be created)
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Please enter a Product Quantity.")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Please enter a UserId.")]
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
