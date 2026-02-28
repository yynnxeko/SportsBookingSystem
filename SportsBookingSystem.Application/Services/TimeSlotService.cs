using AutoMapper;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Services
{
    public class TimeSlotService : ITimeSlotService
    {
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public TimeSlotService(ITimeSlotRepository timeSlotRepository, IMapper mapper)
        {
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        public async Task<List<TimeSlotDto>> GetAllAsync()
        {
            var timeSlots = await _timeSlotRepository.GetAllAsync();
            return _mapper.Map<List<TimeSlotDto>>(timeSlots);
        }

        public async Task<TimeSlotDto?> GetByIdAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            if (timeSlot == null) return null;

            return _mapper.Map<TimeSlotDto>(timeSlot);
        }

        public async Task<TimeSlotDto> CreateAsync(TimeSlotCreateDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
            {
                throw new ArgumentException("StartTime must be before EndTime.");
            }

            var isOverlap = await _timeSlotRepository.IsOverlapAsync(dto.StartTime, dto.EndTime);
            if (isOverlap)
            {
                throw new ArgumentException("The specified time range overlaps with an existing time slot.");
            }

            var timeSlot = _mapper.Map<TimeSlot>(dto);
            await _timeSlotRepository.CreateAsync(timeSlot);

            return _mapper.Map<TimeSlotDto>(timeSlot);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var timeSlot = await _timeSlotRepository.GetByIdAsync(id);
            if (timeSlot == null)
            {
                throw new KeyNotFoundException($"TimeSlot with ID {id} not found.");
            }

            var hasBookings = await _timeSlotRepository.HasBookingsAsync(id);
            if (hasBookings)
            {
                throw new InvalidOperationException("Cannot delete time slot because it has existing bookings.");
            }

            return await _timeSlotRepository.DeleteAsync(id);
        }
    }
}
