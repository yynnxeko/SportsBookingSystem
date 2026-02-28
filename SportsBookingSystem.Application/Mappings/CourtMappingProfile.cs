using AutoMapper;
using SportsBookingSystem.Application.DTOs.CourtDtos;
using SportsBookingSystem.Core.Models;

namespace SportsBookingSystem.Application.Mappings
{
    public class CourtMappingProfile : Profile
    {
        public CourtMappingProfile()
        {
            CreateMap<Court, CourtDto>()
                .ForMember(dest => dest.SportTypeName, opt => opt.MapFrom(src => src.SportType != null ? src.SportType.Name : string.Empty));
                
            CreateMap<CourtCreateDto, Court>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.BookingDetails, opt => opt.Ignore())
                .ForMember(dest => dest.CourtPriceRules, opt => opt.Ignore())
                .ForMember(dest => dest.SportType, opt => opt.Ignore());

            CreateMap<CourtUpdateDto, Court>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.BookingDetails, opt => opt.Ignore())
                .ForMember(dest => dest.CourtPriceRules, opt => opt.Ignore())
                .ForMember(dest => dest.SportType, opt => opt.Ignore());
        }
    }
}
