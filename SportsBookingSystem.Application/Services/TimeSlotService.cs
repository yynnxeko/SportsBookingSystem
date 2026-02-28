using AutoMapper;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
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
    }
}
