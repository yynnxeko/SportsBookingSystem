using SportsBookingSystem.Application.DTOs.BookingDtos;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SportsBookingSystem.Application.Services
{
    public class BookingPriceService : IBookingPriceService
    {
        private readonly ICourtPriceRuleRepository _priceRuleRepository;
        private readonly ITimeSlotRepository _timeSlotRepository;

        public BookingPriceService(
            ICourtPriceRuleRepository priceRuleRepository,
            ITimeSlotRepository timeSlotRepository)
        {
            _priceRuleRepository = priceRuleRepository;
            _timeSlotRepository = timeSlotRepository;
        }

        public async Task<BookingPriceCalculationResultDto> CalculatePriceAsync(CalculateBookingPriceRequestDto request)
        {
            var result = new BookingPriceCalculationResultDto();

            if (request.TimeSlotIds == null || request.TimeSlotIds.Count == 0)
            {
                return result; 
            }

            var rules = await _priceRuleRepository.GetByCourtIdAsync(request.CourtId);
            
            int dayOfWeek = (int)request.Date.DayOfWeek;

            var applicableRules = rules.Where(r => r.DayOfWeek == dayOfWeek).ToList();

            foreach (var slotId in request.TimeSlotIds)
            {
                var slot = await _timeSlotRepository.GetByIdAsync(slotId);
                if (slot == null)
                {
                    throw new ArgumentException($"TimeSlot with ID {slotId} not found.");
                }

                var matchingRule = applicableRules.FirstOrDefault(r => 
                    r.StartTime <= slot.StartTime && r.EndTime >= slot.EndTime);

                decimal priceForSlot = 0;
                string appliedRule = string.Empty;

                if (matchingRule != null)
                {
                    TimeSpan slotDuration = slot.EndTime - slot.StartTime;
                    decimal hours = (decimal)slotDuration.TotalHours;
                    
                    if (hours < 0) hours += 24;

                    priceForSlot = matchingRule.PricePerHour * hours;
                    appliedRule = "Price Rule matched";
                }
                else
                {
                    throw new ArgumentException($"No pricing rule found for Court {request.CourtId} on day {dayOfWeek} at slot {slot.StartTime:HH\\:mm}-{slot.EndTime:HH\\:mm}. Please configure pricing first.");
                }

                result.Breakdown.Add(new BookingPriceBreakdownDto
                {
                    TimeSlotId = slot.Id,
                    SlotDisplayName = $"{slot.StartTime:HH\\:mm} - {slot.EndTime:HH\\:mm}",
                    Price = priceForSlot,
                    AppliedRuleName = appliedRule
                });

                result.TotalPrice += priceForSlot;
            }

            return result;
        }
    }
}
