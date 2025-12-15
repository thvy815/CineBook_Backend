using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeSeatController : ControllerBase
    {
        private readonly ShowtimeSeatService _service;
        public ShowtimeSeatController(ShowtimeSeatService service) => _service = service;

        #region DTOs
        public class LockSeatRequest
        {
            public Guid ShowtimeId { get; set; }
            public Guid SeatId { get; set; }
        }

        public class BookSeatsRequest
        {
            public Guid ShowtimeId { get; set; }
            public List<Guid> SeatIds { get; set; } = new();
        }

        public class GenerateSeatsRequest
        {
            public Guid ShowtimeId { get; set; }
            public int SeatCount { get; set; }
        }
        #endregion

        // Lấy danh sách ghế theo showtime
        [HttpGet("{showtimeId}")]
        public async Task<IActionResult> GetByShowtime([FromRoute] Guid showtimeId)
        {
            var seats = await _service.GetByShowtimeAsync(showtimeId);
            return Ok(seats);
        }

        // Lock ghế tạm thời
        [HttpPost("lock")]
        public async Task<ActionResult> LockSeat([FromBody] LockSeatRequest request)
        {
            var success = await _service.TryLockSeatDb(request.ShowtimeId, request.SeatId);
            if (!success) return BadRequest("Seat is already blocked or booked.");
            return Ok(new { message = "Seat locked successfully" });
        }

        // Release ghế (hủy lock)
        [HttpPost("release")]
        public async Task<ActionResult> ReleaseSeat([FromBody] LockSeatRequest request)
        {
            var success = await _service.ReleaseSeatDb(request.ShowtimeId, request.SeatId);
            if (!success) return BadRequest("Seat is not blocked or already booked.");
            return Ok(new { message = "Seat released successfully" });
        }

        // Book ghế (thanh toán thành công)
        [HttpPost("book")]
        public async Task<ActionResult> BookSeats([FromBody] BookSeatsRequest request)
        {
            foreach (var seatId in request.SeatIds)
            {
                var booked = await _service.BookSeatDb(request.ShowtimeId, seatId);
                if (!booked) return BadRequest($"Seat {seatId} cannot be booked (already released or booked).");
            }
            return Ok(new { message = "Seats booked successfully" });
        }

        // Tạo ghế cho showtime
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateShowtimeSeats([FromBody] GenerateSeatsRequest request)
        {
            var count = await _service.CreateShowtimeSeatsAsync(request.ShowtimeId);
            return Ok(new
            {
                message = "Showtime seats created successfully",
                totalSeats = count
            });
        }
    }
}
