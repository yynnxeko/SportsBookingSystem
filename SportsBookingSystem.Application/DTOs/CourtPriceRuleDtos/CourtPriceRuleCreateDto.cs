using System;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos
{
    public class CourtPriceRuleCreateDto
    {
        [Required]
        public Guid CourtId { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "DayOfWeek must be between 0 (Sunday) and 6 (Saturday)")]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "PricePerHour must be a minimum of 0")]
        public decimal PricePerHour { get; set; }
    }
}
