
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.SportTypeDtos;
using SportsBookingSystem.Application.Interfaces.IService;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportTypeController : ControllerBase
    {
        private readonly ISportTypeService _sportTypeService;

        public SportTypeController(ISportTypeService sportTypeService)
        {
            _sportTypeService = sportTypeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSportType([FromBody] SportTypeCreatedDto sportType)
        {
            try
            {
                var result = await _sportTypeService.AddSportTypeAsync(sportType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSportTypeById(int id)
        {
            try
            {
                var result = await _sportTypeService.GetSportTypeByIdAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSportTypes()
        {
            var result = await _sportTypeService.GetAllSportTypesAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSportType(int id, [FromBody] SportTypeCreatedDto dto)
        {
            try
            {
                var sportType = await _sportTypeService.GetSportTypeByIdAsync(id);
                sportType.Name = dto.Name;
                var result = await _sportTypeService.UpdateSportTypeAsync(sportType);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSportType(int id)
        {
            try
            {
                await _sportTypeService.DeleteSportTypeAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    } 
}
