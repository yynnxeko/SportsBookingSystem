using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction> CreateAsync(PaymentTransaction payment);
        Task<PaymentTransaction?> GetByIdAsync(Guid id);
        Task<IEnumerable<PaymentTransaction>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize);
        Task<int> CountByUserIdAsync(Guid userId);
        Task<IEnumerable<PaymentTransaction>> GetAllPagedAsync(int page, int pageSize);
        Task<int> CountAllAsync();
        Task UpdateAsync(PaymentTransaction payment);
    }
}
