using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;
using System.Collections.Immutable;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeSeatController : ControllerBase
    {
        private readonly ShowtimeSeatService _service;
        public ShowtimeSeatController(ShowtimeSeatService service) => _service = service;

        [HttpGet("{showtimeId}")]
        public async Task<IActionResult> GetByShowtime(Guid showtimeId)
            => Ok(await _service.GetByShowtimeAsync(showtimeId));

        [HttpPost]
        public async Task<IActionResult> Create(CreateShowtimeSeatDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPost("create-for-room")]
        public async Task<IActionResult> CreateForRoom([FromQuery] Guid showtimeId, [FromQuery] Guid roomId)
        {
            var seats = await _service.CreateSeatsForShowtimeAsync(showtimeId, roomId);
            return Ok(seats);
        }

        [HttpPost("{seatId}/lock")]
        public async Task<ActionResult> LockSeat(Guid showtimeId, Guid seatId, [FromQuery] string userId)
        {
            var success = await _service.TryLockSeatDb(showtimeId, seatId, userId);
            if (!success) return BadRequest("Seat is already blocked or booked.");
            return Ok();
        }

        // POST: api/showtimes/{showtimeId}/seats/{seatId}/release
        [HttpPost("{seatId}/release")]
        public async Task<ActionResult> ReleaseSeat(Guid showtimeId, Guid seatId, string userId)
        {
            var success = await _service.ReleaseSeatDb(showtimeId, seatId, userId);
            if (!success) return BadRequest("Seat is not blocked.");
            return Ok();
        }

        // GET: api/showtimes/{showtimeId}/seats/locked
        [HttpGet("locked")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetLockedSeats(Guid showtimeId)
        {
            var lockedSeats = await _service.GetLockedSeatsDb(showtimeId);
            return Ok(lockedSeats);
        }

        // POST: api/showtimes/{showtimeId}/seats/book
        [HttpPost("book")]
        public async Task<ActionResult> BookSeats(Guid showtimeId, [FromBody] List<Guid> seatIds)
        {
            foreach (var seatId in seatIds)
            {
                var locked = await _service.TryLockSeatDb(showtimeId, seatId, "system"); // "system" hoặc userId
                if (!locked) return BadRequest($"Seat {seatId} cannot be booked.");

                // Set trạng thái thực sự là Booked
                await _service.BookSeatDb(showtimeId, seatId);
            }

            return Ok();
        }
    }
}
