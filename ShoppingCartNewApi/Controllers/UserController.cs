using Microsoft.AspNetCore.Mvc;
using ShoppingCartNewApi.Entities;
using ShoppingCartNewApi.Models;
using ShoppingCartNewApi.Services;

namespace ShoppingCartNewApi.Controllers
{
    [Route("shopping")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;

        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(UserDto user)
        {
            var existingUser = await _userService.GetExistingUser(user);
            if (existingUser.UserName == user.UserName)
            {
                return Conflict("User Already exists.");
            }

            var newUser = new User { UserName = user.UserName, Password = user.Password };

            await _userService.AddAndSaveAsync(newUser);

            return Ok("User is successfully registered.");
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(UserDto user)
        {
            var existingUser = await _userService.GetExistingUser(user);
            if (existingUser.UserName == user.UserName && existingUser.Password == user.Password)
            {
                var result = await _userService.LoginUserAsync(existingUser);
                var response = new
                {
                    userId = existingUser.UserId,
                    token = result
                };

                return Ok(response);
            }
            return BadRequest("User is not registered or invalid credentials.");
        }
    }
}