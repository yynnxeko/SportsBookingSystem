using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.CourtDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courts = await _courtService.GetAllAsync();
            return Ok(courts);
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability([FromQuery] int sportTypeId, [FromQuery] DateOnly date)
        {
            var availability = await _courtService.GetAvailabilityAsync(sportTypeId, date);
            return Ok(availability);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var court = await _courtService.GetByIdAsync(id);
            if (court == null)
            {
                return NotFound($"Court with ID {id} not found.");
            }
            return Ok(court);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourtCreateDto dto)
        {
            try
            {
                var createdCourt = await _courtService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdCourt.Id }, createdCourt);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourtUpdateDto dto)
        {
            try
            {
                var updatedCourt = await _courtService.UpdateAsync(id, dto);
                return Ok(updatedCourt);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _courtService.DeleteAsync(id);
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
