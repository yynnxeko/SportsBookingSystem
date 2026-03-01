using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.UserDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreatedDto dto)
        {
            try
            {
                var user = await _userService.CreateAsync(dto);
                return Ok(new { message = "User registered successfully", userId = user.Id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            try
            {
                var token = await _userService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
