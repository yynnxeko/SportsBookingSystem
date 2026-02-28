using System;

namespace SportsBookingSystem.Application.DTOs.CourtDtos
{
    public class CourtDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SportTypeId { get; set; }
        public string SportTypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
