using BookingService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.API.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly BookingBusiness _business;
    public BookingController(BookingBusiness business) => _business = business;

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest req)
    {
        var result = await _business.CreateBookingPending(req);
        return Ok(result);
    }

    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingById(Guid bookingId)
    {
        try
        {
            var booking = await _business.GetBookingById(bookingId);

            if (booking == null)
                return NotFound(new { message = "Booking not found" });

            return Ok(booking);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Internal server error", error = ex.Message });
        }
    }


}
