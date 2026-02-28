using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotController : ControllerBase
    {
        private readonly ITimeSlotService _timeSlotService;

        public TimeSlotController(ITimeSlotService timeSlotService)
        {
            _timeSlotService = timeSlotService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var timeSlots = await _timeSlotService.GetAllAsync();
            return Ok(timeSlots);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var timeSlot = await _timeSlotService.GetByIdAsync(id);
            if (timeSlot == null)
            {
                return NotFound($"TimeSlot with ID {id} not found.");
            }
            return Ok(timeSlot);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimeSlotCreateDto dto)
        {
            try
            {
                var createdSlot = await _timeSlotService.CreateAsync(dto);
                return Ok(createdSlot);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _timeSlotService.DeleteAsync(id);
                return NoContent();
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
