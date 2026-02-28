using SportsBookingSystem.Application.DTOs.BookingDtos;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Interfaces.IService
{
    public interface IBookingPriceService
    {
        Task<BookingPriceCalculationResultDto> CalculatePriceAsync(CalculateBookingPriceRequestDto request);
    }
}
