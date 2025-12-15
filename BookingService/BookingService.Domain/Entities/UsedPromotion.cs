namespace BookingService.Domain.Entities;

public class UsedPromotion
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PromotionCode { get; set; }
    public Guid BookingId { get; set; }
    public DateTime UsedAt { get; set; }
}
