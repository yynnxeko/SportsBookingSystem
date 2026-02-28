using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface ICourtPriceRuleRepository
    {
        Task<List<CourtPriceRule>> GetAllAsync();
        Task<List<CourtPriceRule>> GetByCourtIdAsync(Guid courtId);
        Task<CourtPriceRule?> GetByIdAsync(Guid id);
        Task<CourtPriceRule> CreateAsync(CourtPriceRule rule);
        Task<CourtPriceRule> UpdateAsync(CourtPriceRule rule);
        Task<bool> DeleteAsync(Guid id);
        
        // For validation: check if rule overlaps with existing rules
        Task<bool> IsOverlapAsync(Guid courtId, int dayOfWeek, TimeOnly startTime, TimeOnly endTime, Guid? excludeRuleId = null);

        // Find the matching price rule for a specific court, day, and time slot
        Task<CourtPriceRule?> FindMatchingRuleAsync(Guid courtId, int dayOfWeek, TimeOnly slotStartTime, TimeOnly slotEndTime);
    }
}
