using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using BookingService.Infrastructure.Data;

namespace BookingService.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _db;
    public BookingRepository(BookingDbContext db) => _db = db;

    public async Task<Guid> CreateAsync(
        Booking booking,
        List<BookingSeat> seats,
        BookingPromotion promotion)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        _db.Bookings.Add(booking);
        _db.BookingSeats.AddRange(seats);
        if (promotion != null)
            _db.BookingPromotions.Add(promotion);

        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return booking.Id;
    }
}
