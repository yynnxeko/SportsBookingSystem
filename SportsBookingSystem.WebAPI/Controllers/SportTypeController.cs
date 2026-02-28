
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
            var result = await _sportTypeService.AddSportTypeAsync(sportType);
            if (result == null)
            {
                return BadRequest("Failed to create sport type.");
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSportTypeById(int id)
        {
            var result = await _sportTypeService.GetSportTypeByIdAsync(id);
            if (result == null)
            {
                return NotFound("Sport type not found.");
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSportTypes()
        {
            var result = await _sportTypeService.GetAllSportTypesAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSportType(int id,SportTypeCreatedDto dto)
        {
            var sportType = await _sportTypeService.GetSportTypeByIdAsync(id);
            sportType.Name = dto.Name;
            var result = await _sportTypeService.UpdateSportTypeAsync(sportType);
            if (result == null)
            {
                return NotFound("Sport type not found.");
            }
            return Ok(result);
        }
    } 
}
