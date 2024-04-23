using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNewApi.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Enter a valid UserName")]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter a valid Password")]
        [MaxLength(200)]
        public string Password { get; set; }
    }
}
