using System;
using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Application.DTOs.CourtDtos
{
    public class CourtCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int SportTypeId { get; set; }
    }
}
