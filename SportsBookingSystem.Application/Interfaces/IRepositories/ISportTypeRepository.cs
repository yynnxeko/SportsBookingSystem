using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface ISportTypeRepository
    {
        Task<SportType?> GetSportTypeByIdAsync(int id);
        Task<List<SportType>> GetAllSportTypesAsync();
        Task<SportType?> AddSportTypeAsync(SportType sportType);
        Task<SportType?> UpdateSportTypeAsync(SportType sportType);
        Task<bool?> DeleteSportTypeAsync(int id);
        Task<bool> IsSportTypeExistAsync(string name);  
    }
}
