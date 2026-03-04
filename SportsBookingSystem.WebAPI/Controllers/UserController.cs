using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Interfaces.IService;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
            [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId)) return BadRequest("Invalid User ID");

            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found");


            return Ok(user);
        }
    }
}
