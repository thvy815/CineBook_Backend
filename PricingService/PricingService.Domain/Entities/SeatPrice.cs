public class SeatPrice
{
    public Guid Id { get; set; }
    public string SeatType { get; set; } = null!;
    public string TicketType { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
}
