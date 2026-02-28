using System;

namespace SportsBookingSystem.Application.DTOs.TimeSlotDtos
{
    public class TimeSlotDto
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string DisplayName => $"{StartTime:HH\\:mm} - {EndTime:HH\\:mm}";
    }
}
