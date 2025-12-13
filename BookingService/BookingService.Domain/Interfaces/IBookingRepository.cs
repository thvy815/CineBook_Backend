using BookingService.Domain.Entities;

namespace BookingService.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Guid> CreateAsync(
        Booking booking,
        List<BookingSeat> seats,
        BookingPromotion promotion);
}
