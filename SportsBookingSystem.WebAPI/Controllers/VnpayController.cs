using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SportsBookingSystem.Application.Options;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using System;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly VnpayOptions _options;

        public VnpayController(IVnpay vnPayservice, IOptions<VnpayOptions> options)
        {
            _vnpay = vnPayservice;
            _options = options.Value;

            _vnpay.Initialize(_options.TmnCode, _options.HashSecret, _options.BaseUrl, _options.CallbackUrl);
        }

        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double money, string? description = "Thanh toan dich vu")
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY,
                    CreatedDate = DateTime.Now,
                    Currency = Currency.VND,
                    Language = DisplayLanguage.Vietnamese
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. (IPN Action)
        /// </summary>
        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // TODO: Update PaymentTransaction status in database
                        return Ok();
                    }
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return NotFound("Không tìm thấy thông tin thanh toán.");
        }

        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        [HttpGet("Callback")]
        public IActionResult Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    var frontendUrl = _options.FrontendUrl;
                    var frontendCallbackPath = _options.FrontendCallbackPath;
                    var queryString = Request.QueryString.ToString();
                    var redirectUrl = $"{frontendUrl}{frontendCallbackPath}{queryString}";

                    return Redirect(redirectUrl);
                }
                catch (Exception ex)
                {
                    var frontendUrl = _options.FrontendUrl;
                    var frontendCallbackPath = _options.FrontendCallbackPath;
                    var errorQuery = $"?vnp_ResponseCode=99&error={Uri.EscapeDataString(ex.Message)}";
                    return Redirect($"{frontendUrl}{frontendCallbackPath}{errorQuery}");
                }
            }

            var defaultFrontendUrl = _options.FrontendUrl;
            var defaultFrontendCallbackPath = _options.FrontendCallbackPath;
            return Redirect($"{defaultFrontendUrl}{defaultFrontendCallbackPath}");
        }
    }
}
