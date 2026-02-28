using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<TimeSlot> CreateAsync(TimeSlot timeSlot)
        {
            await _context.TimeSlots.AddAsync(timeSlot);
            await _context.SaveChangesAsync();
            return timeSlot;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var timeSlot = await _context.TimeSlots.FindAsync(id);
            if (timeSlot == null) return false;

            _context.TimeSlots.Remove(timeSlot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HasBookingsAsync(int id)
        {
            return await _context.BookingDetails.AnyAsync(bd => bd.TimeSlotId == id);
        }

        public async Task<bool> IsOverlapAsync(TimeOnly startTime, TimeOnly endTime)
        {
            return await _context.TimeSlots.AnyAsync(t =>
                startTime < t.EndTime && endTime > t.StartTime);
        }
    }
}
