using AutoMapper;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IBookingPriceService _bookingPriceService;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            ICourtRepository courtRepository,
            ITimeSlotRepository timeSlotRepository,
            IBookingPriceService bookingPriceService,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _courtRepository = courtRepository;
            _timeSlotRepository = timeSlotRepository;
            _bookingPriceService = bookingPriceService;
            _mapper = mapper;
        }

        public async Task<BookingDto> CreateBookingAsync(BookingCreateDto dto)
        {
            // 1. Validate Court exists and is active
            var court = await _courtRepository.GetByIdAsync(dto.CourtId);
            if (court == null)
            {
                throw new KeyNotFoundException($"Court with ID {dto.CourtId} not found.");
            }
            if (!court.IsActive)
            {
                throw new InvalidOperationException($"Court '{court.Name}' is not active.");
            }

            // 2. Validate TimeSlots exist
            if (dto.TimeSlotIds == null || dto.TimeSlotIds.Count == 0)
            {
                throw new ArgumentException("At least one time slot must be selected.");
            }

            foreach (var slotId in dto.TimeSlotIds)
            {
                var slot = await _timeSlotRepository.GetByIdAsync(slotId);
                if (slot == null)
                {
                    throw new ArgumentException($"TimeSlot with ID {slotId} not found.");
                }
            }

            // 3. Calculate price using existing BookingPriceService
            var priceRequest = new CalculateBookingPriceRequestDto
            {
                CourtId = dto.CourtId,
                Date = dto.BookingDate,
                TimeSlotIds = dto.TimeSlotIds
            };
            var priceResult = await _bookingPriceService.CalculatePriceAsync(priceRequest);

            // 4. Build Booking entity
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                BookingDate = dto.BookingDate,
                TotalPrice = priceResult.TotalPrice,
                Status = "Confirmed",
                CreatedAt = DateTime.Now
            };

            // 5. Build BookingDetail entities with price from breakdown
            foreach (var breakdown in priceResult.Breakdown)
            {
                booking.BookingDetails.Add(new BookingDetail
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    CourtId = dto.CourtId,
                    TimeSlotId = breakdown.TimeSlotId,
                    BookingDate = dto.BookingDate,
                    PriceAtBookingTime = breakdown.Price
                });
            }

            // 6. Save with transaction + conflict check (handled in repository)
            var createdBooking = await _bookingRepository.CreateAsync(booking);

            // 7. Reload with includes for response mapping
            var result = await _bookingRepository.GetByIdAsync(createdBooking.Id);
            return _mapper.Map<BookingDto>(result);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<List<BookingDto>> GetBookingsByUserIdAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<BookingDto>>(bookings);
        }

        public async Task<BookingDto> CancelBookingAsync(Guid id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found.");
            }

            if (booking.Status == "Cancelled")
            {
                throw new InvalidOperationException("Booking is already cancelled.");
            }

            await _bookingRepository.UpdateStatusAsync(id, "Cancelled");

            // Reload for updated status
            var updatedBooking = await _bookingRepository.GetByIdAsync(id);
            return _mapper.Map<BookingDto>(updatedBooking);
        }
    }
}
