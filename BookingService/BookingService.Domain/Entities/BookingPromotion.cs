namespace BookingService.Domain.Entities;

public class BookingPromotion
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string PromotionCode { get; set; }
    public string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
}
