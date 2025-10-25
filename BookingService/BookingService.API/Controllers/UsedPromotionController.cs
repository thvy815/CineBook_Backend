using BookingService.Application.Services;
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
        public async Task<IActionResult> GetAll() => Ok(await _business.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _business.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UsedPromotion usedPromotion)
        {
            await _business.AddAsync(usedPromotion);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UsedPromotion usedPromotion)
        {
            if (id != usedPromotion.Id) return BadRequest("ID mismatch.");
            await _business.UpdateAsync(usedPromotion);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _business.DeleteAsync(id);
            return Ok();
        }
    }
}
