using System;

namespace SportsBookingSystem.Application.DTOs.UserDtos
{
    public class UpdateWalletBalanceDto
    {
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
    }
}
