using SportsBookingSystem.Application.DTOs.CourtDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface ICourtService
    {
        Task<List<CourtDto>> GetAllAsync();
        Task<CourtDto?> GetByIdAsync(Guid id);
        Task<CourtDto> CreateAsync(CourtCreateDto courtCreateDto);
        Task<CourtDto> UpdateAsync(Guid id, CourtUpdateDto courtUpdateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<List<CourtAvailabilityDto>> GetAvailabilityAsync(int sportTypeId, DateOnly date);
    }
}
