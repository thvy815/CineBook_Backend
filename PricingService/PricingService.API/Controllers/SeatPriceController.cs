using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
using PricingService.Domain.Entities;

namespace PricingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatPriceController : ControllerBase
    {
        private readonly SeatPriceBusiness _business;

        public SeatPriceController(SeatPriceBusiness business)
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
            var seatPrice = await _business.GetByIdAsync(id);
            if (seatPrice == null)
                return NotFound();
            return Ok(seatPrice);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SeatPrice seatPrice)
        {
            await _business.AddAsync(seatPrice);
            return CreatedAtAction(nameof(GetById), new { id = seatPrice.Id }, seatPrice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SeatPrice seatPrice)
        {
            if (id != seatPrice.Id)
                return BadRequest("ID in URL does not match ID in body");

            await _business.UpdateAsync(seatPrice);
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
