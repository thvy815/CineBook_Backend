using BookingService.Application.Services;
using BookingService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingFnbController : ControllerBase
    {
        private readonly BookingFnbBusiness _business;

        public BookingFnbController(BookingFnbBusiness business)
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
        public async Task<IActionResult> Add(BookingFnb bookingFnb)
        {
            await _business.AddAsync(bookingFnb);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, BookingFnb bookingFnb)
        {
            if (id != bookingFnb.Id) return BadRequest("ID mismatch.");
            await _business.UpdateAsync(bookingFnb);
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
