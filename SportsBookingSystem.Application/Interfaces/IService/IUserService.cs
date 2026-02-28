using SportsBookingSystem.Application.DTOs.UserDtos;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserCreatedDto user);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(Guid id);
    }
}
