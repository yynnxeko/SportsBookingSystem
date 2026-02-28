using SportsBookingSystem.Application.DTOs.SportTypeDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Services
{
    public class SportTypeService : ISportTypeService
    {
        private readonly ISportTypeRepository _repo;

        public SportTypeService(ISportTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<SportType> AddSportTypeAsync(SportTypeCreatedDto sportType)
        {
            var existingSportType = await _repo.IsSportTypeExistAsync(sportType.Name);
            if (existingSportType)                
                throw new ArgumentException("Sport type with the same name already exists.");

            var newSportType = new SportType
            {
                Name = sportType.Name
            };

            await _repo.AddSportTypeAsync(newSportType);
            return newSportType;
        }

        public async Task<bool> DeleteSportTypeAsync(int id)
        {
            return await _repo.DeleteSportTypeAsync(id) ?? throw new ArgumentException("Sport type not found.");
        }

        public async Task<IEnumerable<SportType>> GetAllSportTypesAsync()
        {
            return await _repo.GetAllSportTypesAsync();
        }

        public async Task<SportType> GetSportTypeByIdAsync(int id)
        {
            return await _repo.GetSportTypeByIdAsync(id) ?? throw new ArgumentException("Sport type not found.");
        }

        public async Task<SportType> UpdateSportTypeAsync(SportType sportType)
        {
            var updateSportType = await _repo.UpdateSportTypeAsync(sportType);
            if (updateSportType == null)
                throw new ArgumentException("Update failed");

            return updateSportType;
        }
    }
}
