namespace SportsBookingSystem.Application.Options
{
    public class PaymentSettings
    {
        public const string SectionName = "PaymentSettings";
        public int DefaultPageSize { get; set; } = 10;
        public int MaxPageSize { get; set; } = 50;
    }
}
