using SportsBookingSystem.Application.DTOs.SportTypeDtos;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface ISportTypeService
    {
        Task<IEnumerable<SportType>> GetAllSportTypesAsync();
        Task<SportType> GetSportTypeByIdAsync(int id);
        Task<SportType> AddSportTypeAsync(SportTypeCreatedDto sportType);
        Task<SportType> UpdateSportTypeAsync(SportType sportType);
        Task<bool> DeleteSportTypeAsync(int id);
    }
}
