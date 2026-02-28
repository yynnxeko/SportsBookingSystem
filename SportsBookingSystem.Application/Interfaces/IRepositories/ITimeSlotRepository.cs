using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface ITimeSlotRepository
    {
        Task<List<TimeSlot>> GetAllAsync();
        Task<TimeSlot?> GetByIdAsync(int id);
        Task<TimeSlot> CreateAsync(TimeSlot timeSlot);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasBookingsAsync(int id);
        Task<bool> IsOverlapAsync(TimeOnly startTime, TimeOnly endTime);
    }
}
