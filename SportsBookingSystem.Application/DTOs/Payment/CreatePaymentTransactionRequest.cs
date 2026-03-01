using System;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Application.DTOs.Payment
{
    public class CreatePaymentTransactionRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }
        
        public string? PaymentGateway { get; set; }

        [MaxLength(200)]
        public string? TransactionCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
