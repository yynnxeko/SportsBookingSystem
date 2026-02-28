using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingPriceController : ControllerBase
    {
        private readonly IBookingPriceService _bookingPriceService;

        public BookingPriceController(IBookingPriceService bookingPriceService)
        {
            _bookingPriceService = bookingPriceService;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculateBookingPriceRequestDto request)
        {
            try
            {
                var result = await _bookingPriceService.CalculatePriceAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
