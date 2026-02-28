using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Infrastructure.Repositories
{
    public class SportTypeRepository : ISportTypeRepository
    {
        private readonly SportsBookingSystemContext _context;

        public SportTypeRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<SportType?> AddSportTypeAsync(SportType sportType)
        {
            var newSportType = await _context.SportTypes.AddAsync(sportType);
            await _context.SaveChangesAsync();
            return sportType;
        }

        public async Task<bool?> DeleteSportTypeAsync(int id)
        {
            var sportType = await _context.SportTypes.FirstOrDefaultAsync(x => x.Id == id);

            if (sportType == null)
            {               
                return null;
            }

            _context.SportTypes.Remove(sportType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<SportType>> GetAllSportTypesAsync()
        {
            return await _context.SportTypes.ToListAsync();
        }

        public async Task<SportType?> GetSportTypeByIdAsync(int id)
        {
            return await _context.SportTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsSportTypeExistAsync(string name)
        {
            var existingSportType = await _context.SportTypes.FirstOrDefaultAsync(x => x.Name == name);
            if (existingSportType == null) 
                return false;
            
            return true;
        }

        public async Task<SportType?> UpdateSportTypeAsync(SportType sportType)
        {
            var existingSportType = await _context.SportTypes.FirstOrDefaultAsync(x => x.Id == sportType.Id);
            if (existingSportType == null)
                return null;

            existingSportType.Name = sportType.Name;
            await _context.SaveChangesAsync();

            return existingSportType;
        }
    }
}
