using System;
using System.Collections.Generic;

namespace SportsBookingSystem.Application.DTOs.BookingDtos
{
    public class BookingCreateDto
    {
        public Guid UserId { get; set; }
        public Guid CourtId { get; set; }
        public DateOnly BookingDate { get; set; }
        public List<int> TimeSlotIds { get; set; } = new List<int>();
    }
}
