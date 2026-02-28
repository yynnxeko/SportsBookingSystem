using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface ITimeSlotService
    {
        Task<List<TimeSlotDto>> GetAllAsync();
        Task<TimeSlotDto?> GetByIdAsync(int id);
        Task<TimeSlotDto> CreateAsync(TimeSlotCreateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
