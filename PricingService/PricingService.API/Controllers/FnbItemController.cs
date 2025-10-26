using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
using PricingService.Domain.DTOs;
using PricingService.Domain.Entities;

namespace PricingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FnbItemController : ControllerBase
    {
        private readonly FnbItemBusiness _business;

        public FnbItemController(FnbItemBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _business.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _business.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FnbItemDTOs dto)
        {
            await _business.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FnbItemDTOs dto)
        {
            var existing = await _business.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            dto.Id = id;
            await _business.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _business.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _business.DeleteAsync(id);
            return NoContent();
        }
    }
}
