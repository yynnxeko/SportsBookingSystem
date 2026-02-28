using AutoMapper;
using SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Services
{
    public class CourtPriceRuleService : ICourtPriceRuleService
    {
        private readonly ICourtPriceRuleRepository _ruleRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly IMapper _mapper;

        public CourtPriceRuleService(
            ICourtPriceRuleRepository ruleRepository,
            ICourtRepository courtRepository,
            IMapper mapper)
        {
            _ruleRepository = ruleRepository;
            _courtRepository = courtRepository;
            _mapper = mapper;
        }

        public async Task<CourtPriceRuleDto> CreateAsync(CourtPriceRuleCreateDto dto)
        {
            // Validate StartTime < EndTime
            if (dto.StartTime >= dto.EndTime)
            {
                throw new ArgumentException("StartTime must be before EndTime.");
            }

            // Check if Court exists
            var court = await _courtRepository.GetByIdAsync(dto.CourtId);
            if (court == null)
            {
                throw new ArgumentException($"Court with ID {dto.CourtId} not found.");
            }

            // Check for overlaps
            bool isOverlap = await _ruleRepository.IsOverlapAsync(dto.CourtId, dto.DayOfWeek, dto.StartTime, dto.EndTime);
            if (isOverlap)
            {
                throw new ArgumentException("The specified time range overlaps with an existing pricing rule for this court on this day.");
            }

            var rule = _mapper.Map<CourtPriceRule>(dto);
            await _ruleRepository.CreateAsync(rule);
            
            return _mapper.Map<CourtPriceRuleDto>(rule);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null)
            {
                throw new KeyNotFoundException($"CourtPriceRule with ID {id} not found.");
            }

            return await _ruleRepository.DeleteAsync(id);
        }

        public async Task<List<CourtPriceRuleDto>> GetAllAsync()
        {
            var rules = await _ruleRepository.GetAllAsync();
            return _mapper.Map<List<CourtPriceRuleDto>>(rules);
        }

        public async Task<List<CourtPriceRuleDto>> GetByCourtIdAsync(Guid courtId)
        {
            var rules = await _ruleRepository.GetByCourtIdAsync(courtId);
            return _mapper.Map<List<CourtPriceRuleDto>>(rules);
        }

        public async Task<CourtPriceRuleDto?> GetByIdAsync(Guid id)
        {
            var rule = await _ruleRepository.GetByIdAsync(id);
            if (rule == null) return null;

            return _mapper.Map<CourtPriceRuleDto>(rule);
        }

        public async Task<CourtPriceRuleDto> UpdateAsync(Guid id, CourtPriceRuleUpdateDto dto)
        {
            // Validate StartTime < EndTime
            if (dto.StartTime >= dto.EndTime)
            {
                throw new ArgumentException("StartTime must be before EndTime.");
            }

            var existingRule = await _ruleRepository.GetByIdAsync(id);
            if (existingRule == null)
            {
                throw new KeyNotFoundException($"CourtPriceRule with ID {id} not found.");
            }

            // Check for overlaps (excluding current rule)
            bool isOverlap = await _ruleRepository.IsOverlapAsync(existingRule.CourtId, dto.DayOfWeek, dto.StartTime, dto.EndTime, id);
            if (isOverlap)
            {
                throw new ArgumentException("The specified time range overlaps with an existing pricing rule for this court on this day.");
            }

            // Update properties
            existingRule.DayOfWeek = dto.DayOfWeek;
            existingRule.StartTime = dto.StartTime;
            existingRule.EndTime = dto.EndTime;
            existingRule.PricePerHour = dto.PricePerHour;

            await _ruleRepository.UpdateAsync(existingRule);

            return _mapper.Map<CourtPriceRuleDto>(existingRule);
        }
    }
}
