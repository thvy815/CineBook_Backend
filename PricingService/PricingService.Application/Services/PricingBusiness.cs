using Microsoft.EntityFrameworkCore;
using PricingService.Domain.DTOs;
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
        decimal[] seatPrice = new decimal[req.Seats.Count];
        int i = 0;
        // 1️⃣ Tính tiền ghế
        foreach (var seat in req.Seats)
        {
            var price = await _db.SeatPrices.FirstOrDefaultAsync(x =>
                x.SeatType == seat.SeatType &&
                x.TicketType == seat.TicketType);

            if (price == null)
                throw new Exception($"Seat price not found: {seat.SeatType}-{seat.TicketType}");

            seatTotal += price.BasePrice * seat.Quantity;
            seatPrice[i] = price.BasePrice;
            i++;
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

        if (!string.IsNullOrEmpty(req.PromotionCode))
        {
            var promo = await _db.Promotions.FirstOrDefaultAsync(p =>
                p.Code == req.PromotionCode &&
                p.IsActive &&
                p.StartDate <= DateTime.UtcNow &&
                p.EndDate >= DateTime.UtcNow
            );

            if (promo != null)
            {
                discount = promo.DiscountType == "PERCENT"
                    ? subTotal * promo.DiscountValue / 100
                    : promo.DiscountValue;

                // Tạo PromotionDetailDto để trả về
                promoDetail = new PromotionDetailDto(
                        promo.Code,
                        promo.DiscountType,
                        promo.DiscountValue,
                        discount,                // giá trị thực tế đã áp dụng
                        promo.StartDate,
                        promo.EndDate,
                        promo.IsActive,
                        promo.IsOneTimeUse,
                        promo.Description
                    );

            }
        }

        return new CalculatePriceResponse(
            seatPrice,
            fnbTotal,
            subTotal,
            discount,
            subTotal - discount,
            fnbDetails,
        promoDetail
        );
    }

    public async Task<List<G_FNBDTO>> GetAllFnbItemsAsync()
    {
        return await _db.FnbItems
            .Select(f => new G_FNBDTO
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                UnitPrice = f.UnitPrice,
                url = f.url
            })
            .ToListAsync();
    }

    public async Task<List<G_SEATPRICEDTO>> GetAllSeatPricesAsync()
    {
        return await _db.SeatPrices
            .Select(s => new G_SEATPRICEDTO
            {
                Id = s.Id,
                SeatType = s.SeatType,
                TicketType = s.TicketType,
                BasePrice = s.BasePrice,
                Description = s.Description
            })
            .ToListAsync();
    }

    public async Task<List<G_PROMOTION>> GetAllPromotionAsync()
    {
        return await _db.Promotions
            .Select(f => new G_PROMOTION
            {
                Id = f.Id,
                Code = f.Code,
                DiscountType = f.DiscountType,
                DiscountValue = f.DiscountValue,
                IsActive = f.IsActive,
                Description = f.Description
            })
            .ToListAsync();
    }
    public async Task<List<G_PROMOTION>> GetActivePromotions()
    {
        return await _db.Promotions
            .Where(f => f.IsActive)  // Lọc chỉ lấy các promotion có IsActive = true
            .Select(f => new G_PROMOTION
            {
                Id = f.Id,
                Code = f.Code,
                DiscountType = f.DiscountType,
                DiscountValue = f.DiscountValue,
                IsActive = f.IsActive,
                Description = f.Description
            })
            .ToListAsync();
    }


}
