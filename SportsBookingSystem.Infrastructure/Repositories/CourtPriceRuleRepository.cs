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
    public class CourtPriceRuleRepository : ICourtPriceRuleRepository
    {
        private readonly SportsBookingSystemContext _context;

        public CourtPriceRuleRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<CourtPriceRule> CreateAsync(CourtPriceRule rule)
        {
            await _context.CourtPriceRules.AddAsync(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rule = await _context.CourtPriceRules.FindAsync(id);
            if (rule == null) return false;

            _context.CourtPriceRules.Remove(rule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CourtPriceRule>> GetAllAsync()
        {
            return await _context.CourtPriceRules
                .Include(r => r.Court)
                .ToListAsync();
        }

        public async Task<List<CourtPriceRule>> GetByCourtIdAsync(Guid courtId)
        {
            return await _context.CourtPriceRules
                .Include(r => r.Court)
                .Where(r => r.CourtId == courtId)
                .ToListAsync();
        }

        public async Task<CourtPriceRule?> GetByIdAsync(Guid id)
        {
            return await _context.CourtPriceRules
                .Include(r => r.Court)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> IsOverlapAsync(Guid courtId, int dayOfWeek, TimeOnly startTime, TimeOnly endTime, Guid? excludeRuleId = null)
        {
            var query = _context.CourtPriceRules
                .Where(r => r.CourtId == courtId && r.DayOfWeek == dayOfWeek);

            if (excludeRuleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRuleId.Value);
            }

            // Logic overlap: (StartA < EndB) AND (EndA > StartB)
            return await query.AnyAsync(r => 
                startTime < r.EndTime && endTime > r.StartTime);
        }

        public async Task<CourtPriceRule?> FindMatchingRuleAsync(Guid courtId, int dayOfWeek, TimeOnly slotStartTime, TimeOnly slotEndTime)
        {
            return await _context.CourtPriceRules
                .Where(r => r.CourtId == courtId
                         && r.DayOfWeek == dayOfWeek
                         && r.StartTime <= slotStartTime
                         && r.EndTime >= slotEndTime)
                .FirstOrDefaultAsync();
        }

        public async Task<CourtPriceRule> UpdateAsync(CourtPriceRule rule)
        {
            _context.CourtPriceRules.Update(rule);
            await _context.SaveChangesAsync();
            return rule;
        }
    }
}
