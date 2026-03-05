using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SportsBookingSystem.Application.Options;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Core.Models;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpay _vnpay;
        private readonly VnpayOptions _options;
        private readonly IPaymentTransactionRepository _paymentRepo;
        private readonly IWalletTransactionRepository _walletRepo;
        private readonly IUserRepository _userRepo;

        public VnpayController(
            IVnpay vnPayservice, 
            IOptions<VnpayOptions> options,
            IPaymentTransactionRepository paymentRepo,
            IWalletTransactionRepository walletRepo,
            IUserRepository userRepo)
        {
            _vnpay = vnPayservice;
            _options = options.Value;
            _paymentRepo = paymentRepo;
            _walletRepo = walletRepo;
            _userRepo = userRepo;

            _vnpay.Initialize(_options.TmnCode, _options.HashSecret, _options.BaseUrl, _options.CallbackUrl);
        }

        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double money, string userId, string? description = "Thanh toan dich vu")
        {
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Vui lòng cung cấp UserId");
                }

                var generatedDesc = string.IsNullOrEmpty(description) ? "Thanh toan dich vu" : description;
                generatedDesc += $" userId: {userId}";

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = generatedDesc,
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
        public async Task<IActionResult> IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    await ProcessPaymentResultAsync(paymentResult);

                    if (paymentResult.IsSuccess)
                    {
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
        public async Task<IActionResult> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    await ProcessPaymentResultAsync(paymentResult);

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

        private async Task ProcessPaymentResultAsync(PaymentResult paymentResult)
        {
            var transCode = paymentResult.PaymentId.ToString();
            var pt = await _paymentRepo.GetByTransactionCodeAsync(transCode);
            
            // Xử lý nạp tiền theo luồng nạp hệ thống qua CreateDepositUrl (đã có PaymentTransaction)
            if (pt != null && pt.Status == "Pending")
            {
                if (paymentResult.IsSuccess)
                {
                    pt.Status = "Success";
                    await _paymentRepo.UpdateAsync(pt);

                    var user = await _userRepo.GetByIdAsync(pt.UserId);
                    if (user != null)
                    {
                        user.WalletBalance += pt.Amount;
                        await _userRepo.UpdateAsync(user);

                        await _walletRepo.CreateAsync(new WalletTransaction
                        {
                            UserId = user.Id,
                            Amount = pt.Amount,
                            Type = "Deposit",
                            ReferenceId = pt.Id,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
                else
                {
                    pt.Status = "Failed";
                    await _paymentRepo.UpdateAsync(pt);
                }
            }
            // Xử lý trường hợp có giao dịch thành công trả về nhưng chưa được lưu trước đó vào hệ thống
            // Ví dụ người dùng thanh toán qua CreatePaymentUrl mặc định
            else if (pt == null && paymentResult.IsSuccess && Request.Query["vnp_ResponseCode"] == "00")
            {
                var amountRaw = Request.Query["vnp_Amount"].ToString();
                if (double.TryParse(amountRaw, out var amountVal))
                {
                    decimal exactAmount = (decimal)(amountVal / 100);

                    // Trích xuất UserId từ OrderInfo nếu có. Ví dụ "Thanh toan dich vu cho userId: 1234-abcd..."
                    var orderInfo = Request.Query["vnp_OrderInfo"].ToString();
                    Guid targetUserId = Guid.Empty; // Mặc định nếu không tìm thấy UserId
                    
                    // Thử tìm user đang đăng nhập (nhưng callback này thường do server-to-server gọi nên không có Auth context chứa token)
                    // Nếu khách có dùng UserId nhúng vào OrderInfo
                    foreach(var part in orderInfo.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if(Guid.TryParse(part, out Guid parsedId))
                        {
                            targetUserId = parsedId;
                            break;
                        }
                    }

                    if (targetUserId != Guid.Empty)
                    {
                        var user = await _userRepo.GetByIdAsync(targetUserId);
                        if(user != null)
                        {
                            // Tạm tạo 1 bản ghi PaymentTransactions để thoả mãn "luu luon ca lich su chuyen khoan vao PaymentTransactions"
                            var newPayment = new PaymentTransaction
                            {
                                UserId = targetUserId, // Yêu cầu người dùng truyền đúng UserId lên OrderInfo
                                Amount = exactAmount,
                                PaymentGateway = "VNPAY",
                                TransactionCode = transCode,
                                Status = "Success",
                                CreatedAt = DateTime.Now
                            };
                            await _paymentRepo.CreateAsync(newPayment);

                            user.WalletBalance += exactAmount;
                            await _userRepo.UpdateAsync(user);
                            
                            // Lưu lịch sử ví (wallet transaction)
                             await _walletRepo.CreateAsync(new WalletTransaction
                            {
                                UserId = targetUserId,
                                Amount = exactAmount,
                                Type = "Deposit",
                                ReferenceId = newPayment.Id,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tạo url nạp tiền vào ví
        /// </summary>
        [HttpPost("DepositAmount")]
        [Authorize]
        public async Task<ActionResult<string>> CreateDepositUrl([FromBody] double amount)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                    return Unauthorized();

                var user = await _userRepo.GetByIdAsync(userId);
                if (user == null) return NotFound("Người dùng không tồn tại");

                if (amount <= 0) return BadRequest("Số tiền phải lớn hơn 0");

                var ipAddress = NetworkHelper.GetIpAddress(HttpContext);
                long ticks = DateTime.Now.Ticks;

                var pt = new PaymentTransaction
                {
                    UserId = userId,
                    Amount = (decimal)amount,
                    PaymentGateway = "VNPAY",
                    TransactionCode = ticks.ToString(),
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };
                await _paymentRepo.CreateAsync(pt);

                var request = new PaymentRequest
                {
                    PaymentId = ticks,
                    Money = amount,
                    Description = $"Nap tien vao vi Sport Booking {userId}",
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
    }
}
