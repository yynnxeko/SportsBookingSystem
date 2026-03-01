using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Application.DTOs.Payment
{
    public class UpdatePaymentTransactionStatusRequest
    {
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;

        [MaxLength(200)]
        public string? TransactionCode { get; set; }
    }
}
