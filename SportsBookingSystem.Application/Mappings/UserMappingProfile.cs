using AutoMapper;
using SportsBookingSystem.Application.DTOs.UserDtos;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<User, UserDto>();

            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())  
                .ForMember(dest => dest.Bookings, opt => opt.Ignore())     
                .ForMember(dest => dest.PaymentTransactions, opt => opt.Ignore())
                .ForMember(dest => dest.WalletTransactions, opt => opt.Ignore());

            CreateMap<UserCreatedDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())          
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow)) 
            .ForMember(dest => dest.Bookings, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.WalletTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.WalletBalance, opt => opt.MapFrom(_ => 0m)); 

        }

    }
}

