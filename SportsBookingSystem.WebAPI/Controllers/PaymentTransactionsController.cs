using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SportsBookingSystem.Application.DTOs.Payment;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Options;
using SportsBookingSystem.Core.Models;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentTransactionsController : ControllerBase
    {
        private readonly IPaymentTransactionRepository _repo;
        private readonly PaymentSettings _settings;

        public PaymentTransactionsController(IPaymentTransactionRepository repo, IOptions<PaymentSettings> settings)
        {
            _repo = repo;
            _settings = settings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentTransactionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = new PaymentTransaction
            {
                UserId = request.UserId,
                Amount = request.Amount,
                Status = "Pending",
                CreatedAt = request.CreatedAt,
                TransactionCode = request.TransactionCode,
                PaymentGateway = request.PaymentGateway ?? "VNPAY"
            };

            var created = await _repo.CreateAsync(payment);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var payment = await _repo.GetByIdAsync(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByUser(Guid userId, [FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            int p = page ?? 1;
            int ps = pageSize ?? _settings.DefaultPageSize;

            if (p <= 0 || ps <= 0) return BadRequest("Page and PageSize must be greater than 0.");

            var list = await _repo.GetByUserIdPagedAsync(userId, p, ps);
            var totalCount = await _repo.CountByUserIdAsync(userId);

            return Ok(new
            {
                data = list,
                page = p,
                pageSize = ps,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)ps)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? page = null, [FromQuery] int? pageSize = null)
        {
            int p = page ?? 1;
            int ps = pageSize ?? _settings.DefaultPageSize;

            if (p <= 0 || ps <= 0) return BadRequest("Page and PageSize must be greater than 0.");

            var list = await _repo.GetAllPagedAsync(p, ps);
            var totalCount = await _repo.CountAllAsync();

            return Ok(new
            {
                data = list,
                page = p,
                pageSize = ps,
                totalCount,
                totalPages = (int)Math.Ceiling(totalCount / (double)ps)
            });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentTransactionStatusRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status is required.");

            var payment = await _repo.GetByIdAsync(id);
            if (payment == null) return NotFound($"Payment with id {id} not found.");

            payment.Status = request.Status;

            if (!string.IsNullOrWhiteSpace(request.TransactionCode))
                payment.TransactionCode = request.TransactionCode;

            await _repo.UpdateAsync(payment);

            return Ok(payment);
        }
    }
}
