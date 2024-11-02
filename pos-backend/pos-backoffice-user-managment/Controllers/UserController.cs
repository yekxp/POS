using JwtManagerHandler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pos_backoffice_user_managment.Models;
using pos_backoffice_user_managment.Models.DTOs;
using pos_backoffice_user_managment.Services;
using System.ComponentModel.DataAnnotations;

namespace pos_backoffice_user_managment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("role")]
        public async Task<IActionResult> Role([Required] string token)
        {
            string response = await _userService.GetRole(token);

            if (response is null)
                return NotFound();

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest authenticationRequest)
        {
            AuthenticationResponse response = await _userService.Login(authenticationRequest);

            if (response is null)
                return Unauthorized();

            return Ok(response);
        }

        [HttpGet("users")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<User> users = await _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetById(string id)
        {
            User user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequest model)
        {
            await _userService.Create(model);
            return Ok(new { message = "User created" });
        }

        [HttpPut("updateUser/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdate model)
        {
            await _userService.Update(id, model);
            return Ok(new { message = "User updated" });
        }

        [HttpDelete("deleteUser/{id}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userService.Delete(id);
            return Ok(new { message = "User deleted" });
        }
    }
}
