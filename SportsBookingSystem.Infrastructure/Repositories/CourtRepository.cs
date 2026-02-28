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
    public class CourtRepository : ICourtRepository
    {
        private readonly SportsBookingSystemContext _context;

        public CourtRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<Court> CreateAsync(Court court)
        {
            await _context.Courts.AddAsync(court);
            await _context.SaveChangesAsync();
            return court;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var court = await _context.Courts.FindAsync(id);
            if (court == null) return false;

            // Soft delete by updating IsActive flag
            court.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Court>> GetAllAsync()
        {
            return await _context.Courts
                .Include(c => c.SportType)
                .ToListAsync();
        }

        public async Task<List<Court>> GetActiveCourtsBySportTypeAsync(int sportTypeId)
        {
            return await _context.Courts
                .Include(c => c.SportType)
                .Where(c => c.SportTypeId == sportTypeId && c.IsActive)
                .ToListAsync();
        }

        public async Task<List<int>> GetBookedTimeSlotIdsAsync(Guid courtId, DateOnly date)
        {
            return await _context.BookingDetails
                .Where(bd => bd.CourtId == courtId && bd.BookingDate == date)
                .Select(bd => bd.TimeSlotId)
                .ToListAsync();
        }

        public async Task<Court?> GetByIdAsync(Guid id)
        {
            return await _context.Courts
                .Include(c => c.SportType)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> HasBookingsAsync(Guid id)
        {
            return await _context.BookingDetails.AnyAsync(bd => bd.CourtId == id);
        }

        public async Task<Court> UpdateAsync(Court court)
        {
            _context.Courts.Update(court);
            await _context.SaveChangesAsync();
            return court;
        }
    }
}
