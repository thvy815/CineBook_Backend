using BookingService.Application.Services;
using BookingService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly BookingBusiness _bookingBusiness;

        public BookingController(BookingBusiness bookingBusiness)
        {
            _bookingBusiness = bookingBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _bookingBusiness.GetAllAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _bookingBusiness.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Booking booking)
        {
            await _bookingBusiness.AddAsync(booking);
            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Booking booking)
        {
            if (id != booking.Id) return BadRequest("ID mismatch.");
            await _bookingBusiness.UpdateAsync(booking);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _bookingBusiness.DeleteAsync(id);
            return Ok();
        }
    }
}
