using BookingService.Application.Services;
using BookingService.Domain.DTOs;
using BookingService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsedPromotionController : ControllerBase
    {
        private readonly UsedPromotionBusiness _business;

        public UsedPromotionController(UsedPromotionBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsedPromotionDTOs>>> GetAll()
        {
            var items = await _business.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UsedPromotionDTOs>> GetById(Guid id)
        {
            var item = await _business.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] UsedPromotionDTOs dto)
        {
            await _business.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UsedPromotionDTOs dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch.");

            await _business.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _business.DeleteAsync(id);
            return NoContent();
        }
    }
}
