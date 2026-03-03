using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;
using SportsBookingSystem.Infrastructure.Data;
using System.Threading.Tasks;

namespace SportsBookingSystem.Infrastructure.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly SportsBookingSystemContext _context;

        public WalletTransactionRepository(SportsBookingSystemContext context)
        {
            _context = context;
        }

        public async Task<WalletTransaction> CreateAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }
    }
}
