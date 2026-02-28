using Moq;
using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Services;
using SportsBookingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SportsBookingSystem.Application.Tests.Services
{
    public class BookingPriceServiceTests
    {
        private readonly Mock<ICourtPriceRuleRepository> _mockPriceRuleRepo;
        private readonly Mock<ITimeSlotRepository> _mockTimeSlotRepo;
        private readonly BookingPriceService _service;

        public BookingPriceServiceTests()
        {
            _mockPriceRuleRepo = new Mock<ICourtPriceRuleRepository>();
            _mockTimeSlotRepo = new Mock<ITimeSlotRepository>();
            _service = new BookingPriceService(_mockPriceRuleRepo.Object, _mockTimeSlotRepo.Object);
        }

        [Fact]
        public async Task CalculatePriceAsync_MultipleSlotsWithDifferentPrices_ReturnsCorrectTotalAndBreakdown()
        {
            // Arrange
            var courtId = Guid.NewGuid();
            var date = new DateOnly(2026, 3, 2); // Monday (DayOfWeek = 1)

            // Setup Rules
            var rules = new List<CourtPriceRule>
            {
                new CourtPriceRule
                {
                    Id = Guid.NewGuid(),
                    CourtId = courtId,
                    DayOfWeek = 1,
                    StartTime = new TimeOnly(17, 0),
                    EndTime = new TimeOnly(18, 0),
                    PricePerHour = 100000m // 100k
                },
                new CourtPriceRule
                {
                    Id = Guid.NewGuid(),
                    CourtId = courtId,
                    DayOfWeek = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0),
                    PricePerHour = 150000m // 150k
                }
            };

            _mockPriceRuleRepo.Setup(r => r.GetByCourtIdAsync(courtId))
                .ReturnsAsync(rules);

            // Setup TimeSlots
            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new TimeSlot { Id = 1, StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(18, 0) });
            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(2))
                .ReturnsAsync(new TimeSlot { Id = 2, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(19, 0) });

            var request = new CalculateBookingPriceRequestDto
            {
                CourtId = courtId,
                Date = date,
                TimeSlotIds = new List<int> { 1, 2 }
            };

            // Act
            var result = await _service.CalculatePriceAsync(request);

            // Assert
            Assert.Equal(250000m, result.TotalPrice);
            Assert.Equal(2, result.Breakdown.Count);
            
            Assert.Equal(100000m, result.Breakdown[0].Price);
            Assert.Equal(150000m, result.Breakdown[1].Price);
        }

        [Fact]
        public async Task CalculatePriceAsync_NoMatchingRuleFound_ThrowsArgumentException()
        {
            // Arrange
            var courtId = Guid.NewGuid();
            var date = new DateOnly(2026, 3, 3); // Tuesday (DayOfWeek = 2)

            var rules = new List<CourtPriceRule>
            {
                new CourtPriceRule
                {
                    Id = Guid.NewGuid(),
                    CourtId = courtId,
                    DayOfWeek = 1, // Rule only for Monday
                    StartTime = new TimeOnly(17, 0),
                    EndTime = new TimeOnly(18, 0),
                    PricePerHour = 100000m
                }
            };

            _mockPriceRuleRepo.Setup(r => r.GetByCourtIdAsync(courtId))
                .ReturnsAsync(rules);

            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new TimeSlot { Id = 1, StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(18, 0) });

            var request = new CalculateBookingPriceRequestDto
            {
                CourtId = courtId,
                Date = date,
                TimeSlotIds = new List<int> { 1 }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CalculatePriceAsync(request));
            Assert.Contains("No pricing rule found", exception.Message);
        }

        [Fact]
        public async Task CalculatePriceAsync_InvalidTimeSlotId_ThrowsArgumentException()
        {
            // Arrange
            var request = new CalculateBookingPriceRequestDto
            {
                CourtId = Guid.NewGuid(),
                Date = new DateOnly(2026, 3, 2),
                TimeSlotIds = new List<int> { 99 } // Invalid slot
            };

            _mockPriceRuleRepo.Setup(r => r.GetByCourtIdAsync(request.CourtId))
                .ReturnsAsync(new List<CourtPriceRule>());

            _mockTimeSlotRepo.Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((TimeSlot?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.CalculatePriceAsync(request));
            Assert.Contains("not found", exception.Message);
        }
    }
}
