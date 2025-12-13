using Microsoft.EntityFrameworkCore;
using BookingService.Domain.Entities;

namespace BookingService.Infrastructure.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingSeat> BookingSeats => Set<BookingSeat>();
    public DbSet<BookingPromotion> BookingPromotions => Set<BookingPromotion>();
}
