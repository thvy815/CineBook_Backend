using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _business.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FnbItem fnbItem)
        {
            await _business.AddAsync(fnbItem);
            return CreatedAtAction(nameof(GetById), new { id = fnbItem.Id }, fnbItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FnbItem fnbItem)
        {
            if (id != fnbItem.Id)
                return BadRequest("ID mismatch");

            await _business.UpdateAsync(fnbItem);
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
