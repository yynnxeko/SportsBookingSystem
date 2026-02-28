using SportsBookingSystem.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface ITimeSlotRepository
    {
        Task<List<TimeSlot>> GetAllAsync();
        Task<TimeSlot?> GetByIdAsync(int id);
    }
}
