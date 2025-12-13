using BookingService.Application.DTOs;
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
    public async Task<IActionResult> Create(CreateBookingRequest req)
    {
        var id = await _business.CreateBooking(req);
        return Ok(new { bookingId = id });
    }
}
