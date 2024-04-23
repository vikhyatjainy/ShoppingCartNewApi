using Microsoft.IdentityModel.Tokens;
using ShoppingCartNewApi.Data;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCartNewApi.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _conf;
        public UserService(DataContext dataContext, IConfiguration conf)
        {
            _dataContext = dataContext;
            _conf = conf;
        }

        public async Task<User> GetExistingUser(UserDto user)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser == null) return new User();
            return existingUser;
        }

        public async Task AddAndSaveAsync(User user)
        {
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<string> LoginUserAsync(User user)
        {
            // Creating a token
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_conf["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim("given_name", user.UserName),
                new Claim("given_password", user.Password)
            };

            var jwtSecurityToken = new JwtSecurityToken(
                _conf["Authentication:Issuer"],
                _conf["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return tokenToReturn;
        }
    }
}