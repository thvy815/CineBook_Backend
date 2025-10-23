using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
using PricingService.Domain.Entities;

namespace PricingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly PromotionBusiness _business;

        public PromotionController(PromotionBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _business.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var promo = await _business.GetByIdAsync(id);
            if (promo == null)
                return NotFound();
            return Ok(promo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Promotion promotion)
        {
            await _business.AddAsync(promotion);
            return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Promotion promotion)
        {
            if (id != promotion.Id)
                return BadRequest("ID mismatch");

            await _business.UpdateAsync(promotion);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _business.DeleteAsync(id);
            return NoContent();
        }
    }
}
