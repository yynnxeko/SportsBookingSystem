using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using System;
using System.Collections.Generic;

namespace SportsBookingSystem.Application.DTOs.CourtDtos
{
    public class CourtAvailabilityDto
    {
        public Guid CourtId { get; set; }
        public string CourtName { get; set; } = string.Empty;
        public List<SlotAvailabilityDto> Slots { get; set; } = new List<SlotAvailabilityDto>();
    }

    public class SlotAvailabilityDto
    {
        public TimeSlotDto TimeSlot { get; set; } = null!;
        public bool IsAvailable { get; set; }
    }
}
