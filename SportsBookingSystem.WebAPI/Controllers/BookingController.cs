using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid currentUserId))
                return Unauthorized();

            // Nếu người gọi là User bình thường, ép buộc UserId tạo Booking phải là của chính họ.
            // Nếu là Admin thì Admin có quyền tạo thay cho người khác.
            if (roleClaim != "Admin" && dto.UserId != currentUserId)
            {
                return Forbid("You can only create bookings for your own account.");
            }

            try
            {
                var booking = await _bookingService.CreateBookingAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid currentUserId))
                return Unauthorized();

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"Booking with ID {id} not found.");
            }
            if (roleClaim != "Admin" && booking.UserId != currentUserId)
            {
                return Forbid("You do not have permission to view this booking.");
            }

            return Ok(booking);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid currentUserId))
                return Unauthorized();

            if (roleClaim != "Admin" && userId != currentUserId)
            {
                return Forbid("You can only view your own bookings.");
            }

            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid currentUserId))
                return Unauthorized();

            try
            {
                // Check quyền sở hữu Booking trước khi huỷ
                var booking = await _bookingService.GetBookingByIdAsync(id);
                if (booking == null) return NotFound($"Booking with ID {id} not found.");

                if (roleClaim != "Admin" && booking.UserId != currentUserId)
                {
                    return Forbid("You do not have permission to cancel this booking.");
                }

                var canceledBooking = await _bookingService.CancelBookingAsync(id);
                return Ok(canceledBooking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
