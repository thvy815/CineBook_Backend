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


    [HttpPost("{bookingId}/payment")]
    public async Task<IActionResult> CreatePayment(Guid bookingId)
    {
        var orderUrl = await _business.CreatePaymentForBooking(bookingId);
        return Ok(new { orderUrl });
    }

    [HttpPost("api/payments/callback")]
    public async Task<IActionResult> ReceivePaymentCallback([FromBody] PaymentCallbackDto dto)
    {
        Console.WriteLine("Controller callback");
        await _business.HandlePaymentCallback(
            dto.BookingId,
            dto.Status,
            dto.TransactionId,
            dto.PaymentMethod
        );

        return Ok();
    }

    public class PaymentCallbackDto
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } // SUCCESS / FAILED
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
    }



}


