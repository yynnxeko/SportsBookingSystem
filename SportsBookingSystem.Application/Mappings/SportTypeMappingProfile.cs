using AutoMapper;
using SportsBookingSystem.Application.DTOs.SportTypeDtos;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Mappings
{
    public class SportTypeMappingProfile : Profile
    {
        public SportTypeMappingProfile() {
            CreateMap<SportTypeCreatedDto, SportType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.Courts, opt => opt.Ignore());
        }
    }
}
