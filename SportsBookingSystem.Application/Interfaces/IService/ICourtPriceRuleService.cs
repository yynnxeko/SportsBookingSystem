using SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface ICourtPriceRuleService
    {
        Task<List<CourtPriceRuleDto>> GetAllAsync();
        Task<List<CourtPriceRuleDto>> GetByCourtIdAsync(Guid courtId);
        Task<CourtPriceRuleDto?> GetByIdAsync(Guid id);
        Task<CourtPriceRuleDto> CreateAsync(CourtPriceRuleCreateDto dto);
        Task<CourtPriceRuleDto> UpdateAsync(Guid id, CourtPriceRuleUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
