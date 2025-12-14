using Microsoft.EntityFrameworkCore;

public class PricingDbContext : DbContext
{
    public DbSet<SeatPrice> SeatPrices => Set<SeatPrice>();
    public DbSet<FnbItem> FnbItems => Set<FnbItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();

    public PricingDbContext(DbContextOptions<PricingDbContext> options)
        : base(options) { }
}
