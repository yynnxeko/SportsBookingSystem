using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlotDto>> GetAllAsync();
    }
}
