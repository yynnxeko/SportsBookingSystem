using AutoMapper;
using SportsBookingSystem.Application.DTOs.CourtDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;

namespace SportsBookingSystem.Application.Services
{
    public class CourtService : ICourtService
    {
        private readonly ICourtRepository _courtRepository;
        private readonly ISportTypeRepository _sportTypeRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IMapper _mapper;

        public CourtService(
            ICourtRepository courtRepository,
            ISportTypeRepository sportTypeRepository,
            ITimeSlotRepository timeSlotRepository,
            IMapper mapper)
        {
            _courtRepository = courtRepository;
            _sportTypeRepository = sportTypeRepository;
            _timeSlotRepository = timeSlotRepository;
            _mapper = mapper;
        }

        public async Task<CourtDto> CreateAsync(CourtCreateDto courtCreateDto)
        {
            
            var sportType = await _sportTypeRepository.GetSportTypeByIdAsync(courtCreateDto.SportTypeId);
            if (sportType == null)
            {
                throw new ArgumentException($"SportType with ID {courtCreateDto.SportTypeId} does not exist.");
            }

            var court = _mapper.Map<Court>(courtCreateDto);
            
            await _courtRepository.CreateAsync(court);
            
            return _mapper.Map<CourtDto>(court);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var court = await _courtRepository.GetByIdAsync(id);
            if (court == null)
            {
                throw new KeyNotFoundException($"Court with ID {id} not found.");
            }
           
            var hasBookings = await _courtRepository.HasBookingsAsync(id);
            if (hasBookings)
            {
                throw new InvalidOperationException("Cannot delete court because it has existing bookings.");
            }

            return await _courtRepository.DeleteAsync(id);
        }

        public async Task<List<CourtDto>> GetAllAsync()
        {
            var courts = await _courtRepository.GetAllAsync();
            return _mapper.Map<List<CourtDto>>(courts);
        }

        public async Task<CourtDto?> GetByIdAsync(Guid id)
        {
            var court = await _courtRepository.GetByIdAsync(id);
            if (court == null) return null;

            return _mapper.Map<CourtDto>(court);
        }

        public async Task<CourtDto> UpdateAsync(Guid id, CourtUpdateDto courtUpdateDto)
        {
            var sportType = await _sportTypeRepository.GetSportTypeByIdAsync(courtUpdateDto.SportTypeId);
            if (sportType == null)
            {
                throw new ArgumentException($"SportType with ID {courtUpdateDto.SportTypeId} does not exist.");
            }

            var existingCourt = await _courtRepository.GetByIdAsync(id);
            if (existingCourt == null)
            {
                throw new KeyNotFoundException($"Court with ID {id} not found.");
            }

            existingCourt.Name = courtUpdateDto.Name;
            existingCourt.SportTypeId = courtUpdateDto.SportTypeId;
            existingCourt.IsActive = courtUpdateDto.IsActive;

            await _courtRepository.UpdateAsync(existingCourt);

            return _mapper.Map<CourtDto>(existingCourt);
        }
        public async Task<List<CourtAvailabilityDto>> GetAvailabilityAsync(int sportTypeId, DateOnly date)
        {
            var sportCourts = await _courtRepository.GetActiveCourtsBySportTypeAsync(sportTypeId);

            if (sportCourts.Count == 0) return new List<CourtAvailabilityDto>();

            var timeSlots = await _timeSlotRepository.GetAllAsync();
            var timeSlotDtos = _mapper.Map<List<TimeSlotDto>>(timeSlots);

            var courtIds = sportCourts.ConvertAll(c => c.Id);
            var bookedSlotsByCourt = await _courtRepository.GetBookedTimeSlotIdsByCourtAsync(courtIds, date);

            var result = new List<CourtAvailabilityDto>();

            foreach (var court in sportCourts)
            {
                var courtAvailability = new CourtAvailabilityDto
                {
                    CourtId = court.Id,
                    CourtName = court.Name,
                    Slots = new List<SlotAvailabilityDto>()
                };

                var bookedSlotsForThisCourt = bookedSlotsByCourt.ContainsKey(court.Id) 
                    ? bookedSlotsByCourt[court.Id] 
                    : new List<int>();

                foreach (var slot in timeSlotDtos)
                {
                    bool isBooked = bookedSlotsForThisCourt.Contains(slot.Id);
                    
                    courtAvailability.Slots.Add(new SlotAvailabilityDto
                    {
                        TimeSlot = slot,
                        IsAvailable = !isBooked
                    });
                }
                
                result.Add(courtAvailability);
            }

            return result;
        }
    }
}
