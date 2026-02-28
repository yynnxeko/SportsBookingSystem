using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.DTOs.CourtPriceRuleDtos;
using SportsBookingSystem.Application.Interfaces.IService;
using System;
using System.Threading.Tasks;

namespace SportsBookingSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtPriceRuleController : ControllerBase
    {
        private readonly ICourtPriceRuleService _ruleService;

        public CourtPriceRuleController(ICourtPriceRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ruleService.GetAllAsync();
            return Ok(rules);
        }

        [HttpGet("court/{courtId}")]
        public async Task<IActionResult> GetByCourtId(Guid courtId)
        {
            var rules = await _ruleService.GetByCourtIdAsync(courtId);
            return Ok(rules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var rule = await _ruleService.GetByIdAsync(id);
            if (rule == null)
            {
                return NotFound($"CourtPriceRule with ID {id} not found.");
            }
            return Ok(rule);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourtPriceRuleCreateDto dto)
        {
            try
            {
                var createdRule = await _ruleService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdRule.Id }, createdRule);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourtPriceRuleUpdateDto dto)
        {
            try
            {
                var updatedRule = await _ruleService.UpdateAsync(id, dto);
                return Ok(updatedRule);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _ruleService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
