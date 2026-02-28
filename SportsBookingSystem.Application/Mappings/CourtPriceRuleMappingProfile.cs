using AutoMapper;
using SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos;
using SportsBookingSystem.Core.Models;

namespace SportsBookingSystem.Application.Mappings
{
    public class CourtPriceRuleMappingProfile : Profile
    {
        public CourtPriceRuleMappingProfile()
        {
            CreateMap<CourtPriceRule, CourtPriceRuleDto>()
                .ForMember(dest => dest.CourtName, opt => opt.MapFrom(src => src.Court != null ? src.Court.Name : string.Empty));
                
            CreateMap<CourtPriceRuleCreateDto, CourtPriceRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Court, opt => opt.Ignore());

            CreateMap<CourtPriceRuleUpdateDto, CourtPriceRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CourtId, opt => opt.Ignore())
                .ForMember(dest => dest.Court, opt => opt.Ignore());
        }
    }
}
