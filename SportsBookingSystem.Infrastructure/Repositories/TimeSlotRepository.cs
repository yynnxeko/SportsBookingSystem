using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Infrastructure.Repositories
{
    public class TimeSlotRepository : ITimeSlotRepository
    {
        private readonly SportsBookingSystemContext _context;

        public TimeSlotRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<List<TimeSlot>> GetAllAsync()
        {
            return await _context.TimeSlots
                .OrderBy(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<TimeSlot?> GetByIdAsync(int id)
        {
            return await _context.TimeSlots.FindAsync(id);
        }
    }
}
