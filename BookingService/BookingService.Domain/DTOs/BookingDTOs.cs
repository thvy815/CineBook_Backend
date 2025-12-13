namespace BookingService.Application.DTOs;

public class CreateBookingRequest
{
    public Guid UserId { get; set; }
    public Guid ShowtimeId { get; set; }
    public List<SeatDto> Seats { get; set; }
    public string PromotionCode { get; set; }
}

public class SeatDto
{
    public Guid SeatId { get; set; }
    public string SeatType { get; set; }
    public string TicketType { get; set; }
}
