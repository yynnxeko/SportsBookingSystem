using System;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Application.DTOs.TimeSlotDtos
{
    public class TimeSlotCreateDto
    {
        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
