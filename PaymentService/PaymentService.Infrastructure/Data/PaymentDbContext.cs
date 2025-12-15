using Microsoft.EntityFrameworkCore;
namespace PaymentService.Infrastructure.Repositories;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }
    public DbSet<Payment> Payments { get; set; }
}
