using BookingService.Application.Services;
using BookingService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingSeatController : ControllerBase
    {
        private readonly BookingSeatBusiness _business;

        public BookingSeatController(BookingSeatBusiness business)
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
        public async Task<IActionResult> Add(BookingSeat seat)
        {
            await _business.AddAsync(seat);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, BookingSeat seat)
        {
            if (id != seat.Id) return BadRequest("ID mismatch.");
            await _business.UpdateAsync(seat);
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
