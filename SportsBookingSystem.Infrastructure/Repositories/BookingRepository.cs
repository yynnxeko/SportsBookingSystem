using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SportsBookingSystem.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly SportsBookingSystemContext _context;

        public BookingRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            // Sử dụng Serializable isolation level để chống double booking
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Kiểm tra conflict trong transaction (với lock)
                var timeSlotIds = booking.BookingDetails.Select(bd => bd.TimeSlotId).ToList();
                var bookingDate = booking.BookingDetails.First().BookingDate;
                var courtId = booking.BookingDetails.First().CourtId;

                var hasConflict = await _context.BookingDetails
                    .Where(bd => bd.CourtId == courtId
                        && bd.BookingDate == bookingDate
                        && timeSlotIds.Contains(bd.TimeSlotId)
                        && bd.Booking.Status != "Cancelled")
                    .AnyAsync();

                if (hasConflict)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException("One or more selected time slots are already booked for this court on the selected date.");
                }

                // Insert Booking + BookingDetails
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Court)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.TimeSlot)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Court)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.TimeSlot)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(Guid id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found.");
            }

            booking.Status = status;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasConflictAsync(Guid courtId, DateOnly date, List<int> timeSlotIds)
        {
            return await _context.BookingDetails
                .Where(bd => bd.CourtId == courtId
                    && bd.BookingDate == date
                    && timeSlotIds.Contains(bd.TimeSlotId)
                    && bd.Booking.Status != "Cancelled")
                .AnyAsync();
        }
    }
}
