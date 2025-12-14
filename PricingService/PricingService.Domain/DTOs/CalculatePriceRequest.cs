using PricingService.Domain.DTOs;

namespace PricingService.API.DTOs;

public class CalculatePriceRequest
{
    public List<SeatPriceDTOs> Seats { get; set; } = [];
    public List<FnbItemDTOs> Fnbs { get; set; } = [];
    public string? PromotionCode { get; set; }
}
