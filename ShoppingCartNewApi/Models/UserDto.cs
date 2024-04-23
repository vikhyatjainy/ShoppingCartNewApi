using System.ComponentModel.DataAnnotations;

namespace ShoppingCartNewApi.Models
{
    public class UserDto
    {
        [Required(ErrorMessage = "Enter a valid UserName")]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter a valid Password")]
        [MinLength(4)]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
