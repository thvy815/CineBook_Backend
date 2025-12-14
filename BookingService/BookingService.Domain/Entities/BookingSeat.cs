namespace BookingService.Domain.Entities;

public class BookingSeat
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid SeatId { get; set; }
    public string SeatType { get; set; }
    public string TicketType { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}
