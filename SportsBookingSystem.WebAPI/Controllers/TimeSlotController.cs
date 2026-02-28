using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Interfaces.IService;
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
    }
}
