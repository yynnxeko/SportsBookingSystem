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

                    if (!string.IsNullOrEmpty(_options.FrontendUrl))
                    {
                        var frontendUrl = _options.FrontendUrl;
                        var frontendCallbackPath = _options.FrontendCallbackPath;
                        var queryString = Request.QueryString.ToString();
                        var redirectUrl = $"{frontendUrl}{frontendCallbackPath}{queryString}";
                        return Redirect(redirectUrl);
                    }

                    // Provide an HTML view for the Callback since FrontendUrl is not set
                    var isSuccess = paymentResult.IsSuccess;
                    var title = isSuccess ? "Thanh toán thành công!" : "Thanh toán thất bại!";
                    var message = isSuccess 
                        ? "Giao dịch của bạn đã được xử lý thành công. Cảm ơn bạn!" 
                        : $"Giao dịch thất bại. Lý do: {paymentResult.PaymentResponse?.Description ?? "Lỗi không xác định"}";
                    var icon = isSuccess ? "✅" : "❌";
                    var color = isSuccess ? "#28a745" : "#dc3545";

                    var amountRaw = Request.Query["vnp_Amount"].ToString();
                    var amountStr = amountRaw;
                    if (double.TryParse(amountRaw, out var amount))
                    {
                        amountStr = (amount / 100).ToString("N0") + " VNĐ";
                    }

                    var html = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Kết quả thanh toán</title>
                        <style>
                            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; background-color: #f8f9fa; margin: 0; }}
                            .container {{ text-align: center; background: white; padding: 40px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); max-width: 500px; width: 100%; }}
                            .icon {{ font-size: 80px; margin-bottom: 20px; }}
                            h1 {{ color: {color}; margin-bottom: 10px; font-size: 28px; }}
                            p {{ color: #6c757d; font-size: 16px; margin-bottom: 30px; line-height: 1.5; }}
                            .btn {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; transition: background-color 0.3s; }}
                            .btn:hover {{ background-color: #0056b3; }}
                            .details {{ text-align: left; margin-top: 20px; padding: 15px; background-color: #f1f3f5; border-radius: 8px; }}
                            .details p {{ margin: 8px 0; font-size: 14px; color: #495057; display: flex; justify-content: space-between; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='icon'>{icon}</div>
                            <h1>{title}</h1>
                            <p>{message}</p>
                            <div class='details'>
                                <p><strong>Mã đơn hàng:</strong> <span>{paymentResult.PaymentId}</span></p>
                                <p><strong>Mã giao dịch VNPAY:</strong> <span>{paymentResult.VnpayTransactionId}</span></p>
                                <p><strong>Số tiền:</strong> <span>{amountStr}</span></p>
                                <p><strong>Mã phản hồi:</strong> <span>{paymentResult.PaymentResponse?.Code}</span></p>
                            </div>
                            <br/>
                            <a href='/' class='btn'>Về trang chủ</a>
                        </div>
                    </body>
                    </html>";

                    return Content(html, "text/html");
                }
                catch (Exception ex)
                {
                    if (!string.IsNullOrEmpty(_options.FrontendUrl))
                    {
                        var frontendUrl = _options.FrontendUrl;
                        var frontendCallbackPath = _options.FrontendCallbackPath;
                        var errorQuery = $"?vnp_ResponseCode=99&error={Uri.EscapeDataString(ex.Message)}";
                        return Redirect($"{frontendUrl}{frontendCallbackPath}{errorQuery}");
                    }

                    string errorHtml = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <title>Đã xảy ra lỗi</title>
                        <style>
                            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; background-color: #f8f9fa; margin: 0; }}
                            .container {{ text-align: center; background: white; padding: 40px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); max-width: 500px; width: 100%; }}
                            h1 {{ color: #dc3545; }}
                            .btn {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 20px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h1>Đã xảy ra lỗi hệ thống</h1>
                            <p>{ex.Message}</p>
                            <a href='/' class='btn'>Về trang chủ</a>
                        </div>
                    </body>
                    </html>";
                    return Content(errorHtml, "text/html");
                }
            }

            return Content("<html><body><h1 style='text-align:center;font-family:sans-serif;margin-top:50px;'>Không tìm thấy thông tin thanh toán.</h1></body></html>", "text/html");
        }
    }
}
