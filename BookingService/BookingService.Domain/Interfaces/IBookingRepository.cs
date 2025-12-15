using BookingService.Domain.Entities;

namespace BookingService.Domain.Interfaces;

public interface IBookingRepository
{
    /// <summary>
    /// Tạo booking + seats + promotion
    /// </summary>
    Task<Guid> CreateAsync(Booking booking, List<BookingSeat> seats, BookingPromotion promotion, List<BookingFnb> bookingFnb);

    /// <summary>
    /// Cập nhật trạng thái booking + transactionId + paymentMethod
    /// </summary>
    Task UpdateStatusAsync(Guid bookingId, string status, string transactionId, string paymentMethod);

    /// <summary>
    /// Lấy booking theo Id (kèm seats + promotion)
    /// </summary>
    Task<Booking> GetByIdAsync(Guid bookingId);
    Task<List<Booking>> GetPendingBookingsAsync();

    Task<Booking?> GetByTransactionIdAsync(string transactionId);

}
