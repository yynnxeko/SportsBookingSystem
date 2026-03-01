using AutoMapper;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Core.Models;

namespace SportsBookingSystem.Application.Mappings
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            CreateMap<Booking, BookingDto>();

            CreateMap<BookingDetail, BookingDetailDto>()
                .ForMember(dest => dest.CourtName,
                    opt => opt.MapFrom(src => src.Court != null ? src.Court.Name : string.Empty))
                .ForMember(dest => dest.TimeSlotDisplay,
                    opt => opt.MapFrom(src => src.TimeSlot != null
                        ? $"{src.TimeSlot.StartTime:HH\\:mm} - {src.TimeSlot.EndTime:HH\\:mm}"
                        : string.Empty));
        }
    }
}
