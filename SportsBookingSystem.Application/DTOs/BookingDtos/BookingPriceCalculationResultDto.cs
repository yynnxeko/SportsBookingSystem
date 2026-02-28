using System;
using System.Collections.Generic;

namespace SportsBookingSystem.Application.DTOs.BookingDtos
{
    public class BookingPriceCalculationResultDto
    {
        public decimal TotalPrice { get; set; }
        public List<BookingPriceBreakdownDto> Breakdown { get; set; } = new List<BookingPriceBreakdownDto>();
    }

    public class BookingPriceBreakdownDto
    {
        public int TimeSlotId { get; set; }
        public string SlotDisplayName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string AppliedRuleName { get; set; } = string.Empty;
    }

    public class CalculateBookingPriceRequestDto
    {
        public Guid CourtId { get; set; }
        public DateOnly Date { get; set; }
        public List<int> TimeSlotIds { get; set; } = new List<int>();
    }
}
