using SportsBookingSystem.Core.Models;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IRepositories
{
    public interface IWalletTransactionRepository
    {
        Task<WalletTransaction> CreateAsync(WalletTransaction transaction);
    }
}
