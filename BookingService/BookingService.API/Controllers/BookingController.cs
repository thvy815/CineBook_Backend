using BookingService.Application.Services;
using BookingService.Domain.DTOs;
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
        public async Task<ActionResult<IEnumerable<BookingDTOs>>> GetAll()
        {
            var bookings = await _bookingBusiness.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookingDTOs>> GetById(Guid id)
        {
            var booking = await _bookingBusiness.GetByIdAsync(id);
            if (booking == null)
                return NotFound();
            return Ok(booking);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BookingDTOs bookingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _bookingBusiness.AddAsync(bookingDto);
            return CreatedAtAction(nameof(GetById), new { id = bookingDto.Id }, bookingDto);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] BookingDTOs bookingDto)
        {
            if (id != bookingDto.Id)
                return BadRequest("ID mismatch.");

            await _bookingBusiness.UpdateAsync(bookingDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _bookingBusiness.DeleteAsync(id);
            return NoContent();
        }
    }
}
