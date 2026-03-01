namespace SportsBookingSystem.Application.Options
{
    public class VnpayOptions
    {
        public const string Vnpay = "Vnpay";

        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;

        public string FrontendUrl { get; set; } = string.Empty;
        public string FrontendCallbackPath { get; set; } = string.Empty;
    }
}
