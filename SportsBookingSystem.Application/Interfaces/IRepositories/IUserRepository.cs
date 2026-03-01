using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> IsEmailExistedAsync(string email);
        Task<User?> GetByEmailAsync(string email);
    }
}
