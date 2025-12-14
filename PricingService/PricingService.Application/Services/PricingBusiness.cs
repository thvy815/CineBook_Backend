using Microsoft.EntityFrameworkCore;
namespace PricingService.Application.Services;
public class PricingBusiness
{
    private readonly PricingDbContext _db;

    public PricingBusiness(PricingDbContext db)
    {
        _db = db;
    }

    public async Task<CalculatePriceResponse> CalculateAsync(CalculatePriceRequest req)
    {
        decimal seatTotal = 0;
        decimal fnbTotal = 0;

        // 1️⃣ Tính tiền ghế
        foreach (var seat in req.Seats)
        {
            var price = await _db.SeatPrices.FirstOrDefaultAsync(x =>
                x.SeatType == seat.SeatType &&
                x.TicketType == seat.TicketType);

            if (price == null)
                throw new Exception($"Seat price not found: {seat.SeatType}-{seat.TicketType}");

            seatTotal += price.BasePrice * seat.Quantity;
        }

        // 2️⃣ Tính FnB
        var fnbDetails = new List<FnbRequest>();

        foreach (var fnb in req.Fnbs)
        {
            var item = await _db.FnbItems.FindAsync(fnb.FnbItemId)
                ?? throw new Exception("FnB item not found");

            fnbTotal += item.UnitPrice * fnb.Quantity;
            fnbDetails.Add(new FnbRequest(item.Id, item.Name, item.UnitPrice, fnb.Quantity));
        }
        var subTotal = seatTotal + fnbTotal;

        // 3️⃣ Promotion
        decimal discount = 0;
        PromotionDetailDto? promoDetail = null;
        if (req.Promotion != null)
        {
            var promo = await _db.Promotions.FirstOrDefaultAsync(p =>
                p.Code == req.Promotion.Code &&
                p.IsActive &&
                p.StartDate <= DateTime.UtcNow &&
                p.EndDate >= DateTime.UtcNow);

            if (promo != null)
            {
                discount = promo.DiscountType == "PERCENT"
                    ? subTotal * promo.DiscountValue / 100
                    : promo.DiscountValue;

                promoDetail = new PromotionDetailDto(
                    promo.Code,
                    promo.DiscountType,
                    promo.DiscountValue,
                    promo.StartDate,
                    promo.EndDate,
                    promo.IsActive,
                    promo.IsOneTimeUse,
                    promo.Description
                );
            }
        }

        return new CalculatePriceResponse(
            seatTotal,
            fnbTotal,
            subTotal,
            discount,
            subTotal - discount,
            fnbDetails,
            promoDetail
        );
    }
}
