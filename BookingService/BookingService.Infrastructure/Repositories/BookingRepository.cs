using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _db;
    public BookingRepository(BookingDbContext db) => _db = db;

    /// <summary>
    /// Tạo booking cùng seat, promotion, và Fnb
    /// </summary>
    public async Task<Guid> CreateAsync(
    Booking booking,
    List<BookingSeat> seats,
    BookingPromotion promotion,
    List<BookingFnb> fnbs = null)
    {
        using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            _db.Bookings.Add(booking);
            _db.BookingSeats.AddRange(seats);
            if (promotion != null) _db.BookingPromotions.Add(promotion);
            if (fnbs != null && fnbs.Count > 0) _db.BookingFnB.AddRange(fnbs);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return booking.Id;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }


    public async Task UpdateStatusAsync(Guid bookingId, string status, string transactionId, string paymentMethod)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null)
            throw new KeyNotFoundException($"Booking {bookingId} not found.");

        booking.Status = status;
        booking.TransactionId = transactionId;
        booking.PaymentMethod = paymentMethod;
        booking.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Lấy booking theo Id
    /// </summary>
    public async Task<Booking> GetByIdAsync(Guid bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Seats)
            .Include(b => b.Promotion)
            .Include(b => b.BookingFnb)
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
            throw new KeyNotFoundException($"Booking {bookingId} not found.");

        return booking;
    }
    public async Task<Booking?> GetByTransactionIdAsync(string transactionId)
    {
        return await _db.Bookings
            .Include(b => b.Seats)   // ⚠️ CỰC KỲ QUAN TRỌNG
            .FirstOrDefaultAsync(b => b.TransactionId == transactionId);
    }
    public async Task<List<Booking>> GetPendingBookingsAsync()
    {
        return await _db.Bookings
            .Include(b => b.Seats)
            .Where(b => b.Status == "PENDING")
            .ToListAsync();
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _db.Bookings
            .Include(b => b.Seats)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }


}
