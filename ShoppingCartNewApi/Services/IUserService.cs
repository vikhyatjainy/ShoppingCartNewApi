using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;

namespace ShoppingCartNewApi.Services
{
    public interface IUserService
    {
        Task<User> GetExistingUser(UserDto user);
        
        Task AddAndSaveAsync(User user);

        Task<string> LoginUserAsync(User user);
    }
}