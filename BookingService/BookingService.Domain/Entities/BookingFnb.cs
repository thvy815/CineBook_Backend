namespace BookingService.Domain.Entities;

public class BookingFnb
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid FnbItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalFnbPrice { get; set; }
}
