using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface IBookingRepository
    {
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking?> GetByIdAsync(Guid id);
        Task<List<Booking>> GetByUserIdAsync(Guid userId);
        Task UpdateStatusAsync(Guid id, string status);
        Task<bool> HasConflictAsync(Guid courtId, DateOnly date, List<int> timeSlotIds);
    }
}
