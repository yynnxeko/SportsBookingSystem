using AutoMapper;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.DTOs.CourtDtos;
using SportsBookingSystem.Application.DTOs.TimeSlotDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportsBookingSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;
        private readonly IBookingPriceService _bookingPriceService;
        private readonly IUserRepository _userRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IMapper _mapper;

        public BookingService(
            IBookingRepository bookingRepository,
            ICourtRepository courtRepository,
            ITimeSlotRepository timeSlotRepository,
            IBookingPriceService bookingPriceService,
            IUserRepository userRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _courtRepository = courtRepository;
            _timeSlotRepository = timeSlotRepository;
            _bookingPriceService = bookingPriceService;
            _userRepository = userRepository;
            _walletTransactionRepository = walletTransactionRepository;
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

            // 2. Validate TimeSlots exist and available
            if (dto.TimeSlotIds == null || dto.TimeSlotIds.Count == 0)
            {
                throw new ArgumentException("At least one time slot must be selected.");
            }

            var slotsBooked = await _courtRepository.GetBookedTimeSlotIdsAsync(court.Id, dto.BookingDate);
            
            foreach (var slotId in dto.TimeSlotIds)
            {
                var slot = await _timeSlotRepository.GetByIdAsync(slotId);
                if (slot == null)
                {
                    throw new ArgumentException($"TimeSlot with ID {slotId} not found.");
                }

                if (slotsBooked.Contains(slotId))
                {
                    throw new InvalidOperationException(
                        $"TimeSlot {slot.StartTime:HH\\:mm}-{slot.EndTime:HH\\:mm} is already booked for the selected date.");
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

            // 4. Validate user exists and check wallet balance
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {dto.UserId} not found.");
            }

            if (user.WalletBalance < priceResult.TotalPrice)
            {
                throw new InvalidOperationException(
                    $"Insufficient wallet balance. Required: {priceResult.TotalPrice:N0} VND, Available: {user.WalletBalance:N0} VND.");
            }

            // 5. Build Booking entity (initially Pending until payment succeeds)
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                BookingDate = dto.BookingDate,
                TotalPrice = priceResult.TotalPrice,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            // 6. Build BookingDetail entities with price from breakdown
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

            // 7. Save booking with conflict check (handled in repository)
            var createdBooking = await _bookingRepository.CreateAsync(booking);

            // 8. Deduct wallet balance
            user.WalletBalance -= priceResult.TotalPrice;
            await _userRepository.UpdateAsync(user);

            // 9. Record wallet transaction for the payment
            var walletTransaction = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Amount = -priceResult.TotalPrice,
                Type = "BookingPayment",
                ReferenceId = createdBooking.Id,
                CreatedAt = DateTime.Now
            };
            await _walletTransactionRepository.CreateAsync(walletTransaction);

            // 10. Update booking status to Confirmed after successful payment
            await _bookingRepository.UpdateStatusAsync(createdBooking.Id, "Confirmed");

            // 11. Reload with includes for response mapping
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

            // Refund wallet balance to user
            var user = await _userRepository.GetByIdAsync(booking.UserId);
            if (user != null)
            {
                user.WalletBalance += booking.TotalPrice;
                await _userRepository.UpdateAsync(user);

                // Record refund wallet transaction
                var walletTransaction = new WalletTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = booking.UserId,
                    Amount = booking.TotalPrice,
                    Type = "BookingRefund",
                    ReferenceId = booking.Id,
                    CreatedAt = DateTime.Now
                };
                await _walletTransactionRepository.CreateAsync(walletTransaction);
            }

            await _bookingRepository.UpdateStatusAsync(id, "Cancelled");

            // Reload for updated status
            var updatedBooking = await _bookingRepository.GetByIdAsync(id);
            return _mapper.Map<BookingDto>(updatedBooking);
        }
    }
}
