using Microsoft.EntityFrameworkCore;


namespace PricingService.Infrastructure.Repositories;

public class PricingRepository
{
    private readonly PricingDbContext _context;

    public PricingRepository(PricingDbContext context)
    {
        _context = context;
    }

    // 🎟 Seat price
    public async Task<SeatPrice?> GetSeatPriceAsync(string seatType, string ticketType)
    {
        return await _context.SeatPrices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.SeatType == seatType &&
                x.TicketType == ticketType);
    }

    // 🍿 FNB
    public async Task<FnbItem?> GetFnbItemAsync(Guid id)
    {
        return await _context.FnbItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    // 🎁 Promotion – CHECK ĐỦ ĐIỀU KIỆN
    public async Task<Promotion?> GetValidPromotionAsync(string code)
    {
        var now = DateTime.UtcNow;

        return await _context.Promotions
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Code == code &&
                x.IsActive &&
                x.StartDate <= now &&
                x.EndDate >= now);
    }
}
