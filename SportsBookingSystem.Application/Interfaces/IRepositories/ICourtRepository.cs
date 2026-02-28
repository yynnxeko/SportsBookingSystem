using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface ICourtRepository
    {
        Task<List<Court>> GetAllAsync();
        Task<Court?> GetByIdAsync(Guid id);
        Task<Court> CreateAsync(Court court);
        Task<Court> UpdateAsync(Court court);
        Task<bool> HasBookingsAsync(Guid id);
        Task<bool> DeleteAsync(Guid id); // Soft delete or hard delete based on requirements
        
        Task<List<Court>> GetActiveCourtsBySportTypeAsync(int sportTypeId);
        Task<List<int>> GetBookedTimeSlotIdsAsync(Guid courtId, DateOnly date);
    }
}
