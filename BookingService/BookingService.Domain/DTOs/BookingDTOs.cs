public class CreateBookingRequest
{
    public Guid UserId { get; set; }
    public Guid ShowtimeId { get; set; }

    public List<SeatDto> Seats { get; set; } = new List<SeatDto>();

    public string? PromotionCode { get; set; }

    public List<BookingFnbReqDTOs> FnBs { get; set; } = new List<BookingFnbReqDTOs>();


}


public class SeatDto
{
    public Guid ShowtimeSeatId { get; set; }
    public string SeatType { get; set; }
    public string TicketType { get; set; }
    public int Quantity { get; set; }
}

public class BookingFnbReqDTOs
{
    public Guid FnbItemId { get; set; }
    public int Quantity { get; set; }
}


