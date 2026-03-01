using SportsBookingSystem.Application.DTOs.BookingDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(BookingCreateDto dto);
        Task<BookingDto?> GetBookingByIdAsync(Guid id);
        Task<List<BookingDto>> GetBookingsByUserIdAsync(Guid userId);
        Task<BookingDto> CancelBookingAsync(Guid id);
    }
}
