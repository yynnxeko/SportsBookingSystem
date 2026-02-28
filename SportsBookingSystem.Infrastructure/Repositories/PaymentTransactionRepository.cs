using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsBookingSystem.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly SportsBookingSystemContext _context;

        public PaymentTransactionRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<int> CountAllAsync()
        {
            return await _context.PaymentTransactions.CountAsync();
        }

        public async Task<int> CountByUserIdAsync(Guid userId)
        {
            return await _context.PaymentTransactions.CountAsync(p => p.UserId == userId);
        }

        public async Task<PaymentTransaction> CreateAsync(PaymentTransaction payment)
        {
            await _context.PaymentTransactions.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<PaymentTransaction>> GetAllPagedAsync(int page, int pageSize)
        {
            return await _context.PaymentTransactions
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PaymentTransaction?> GetByIdAsync(Guid id)
        {
            return await _context.PaymentTransactions.FindAsync(id);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByUserIdPagedAsync(Guid userId, int page, int pageSize)
        {
            return await _context.PaymentTransactions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync(PaymentTransaction payment)
        {
            _context.PaymentTransactions.Update(payment);
            await _context.SaveChangesAsync();
        }
    }
}
