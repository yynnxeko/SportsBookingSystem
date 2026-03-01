using System;
using System.Collections.Generic;

namespace SportsBookingSystem.Application.DTOs.BookingDtos
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateOnly BookingDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; } = new List<BookingDetailDto>();
    }

    public class BookingDetailDto
    {
        public Guid Id { get; set; }
        public Guid CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public int TimeSlotId { get; set; }
        public string TimeSlotDisplay { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public decimal PriceAtBookingTime { get; set; }
    }
}
