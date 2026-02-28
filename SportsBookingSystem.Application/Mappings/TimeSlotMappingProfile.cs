using AutoMapper;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using SportsBookingSystem.Core.Models;

namespace SportsBookingSystem.Application.Mappings
{
    public class TimeSlotMappingProfile : Profile
    {
        public TimeSlotMappingProfile()
        {
            CreateMap<TimeSlot, TimeSlotDto>();
            CreateMap<TimeSlotCreateDto, TimeSlot>();
        }
    }
}
