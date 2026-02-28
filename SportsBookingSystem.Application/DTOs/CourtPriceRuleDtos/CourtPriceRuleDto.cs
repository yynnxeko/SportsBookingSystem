using System;

namespace SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos
{
    public class CourtPriceRuleDto
    {
        public Guid Id { get; set; }
        public Guid CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public int DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ..., 6 = Saturday
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal PricePerHour { get; set; }
    }
}
