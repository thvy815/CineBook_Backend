using BookingService.Application.Clients;
using BookingService.Application.DTOs;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;

namespace BookingService.Application.Services;

public class BookingBusiness
{
    private readonly ShowtimeSeatClient _seatClient;
    private readonly PricingClient _pricing;
    private readonly IBookingRepository _repo;

    public BookingBusiness(
        ShowtimeSeatClient seatClient,
        PricingClient pricing,
        IBookingRepository repo)
    {
        _seatClient = seatClient;
        _pricing = pricing;
        _repo = repo;
    }

    public async Task<Guid> CreateBooking(CreateBookingRequest req)
    {
        var locked = new List<Guid>();

        try
        {
            foreach (var s in req.Seats)
            {
                var ok = await _seatClient.LockSeat(
                    req.ShowtimeId, s.SeatId, req.UserId.ToString());

                if (!ok) throw new Exception("Seat locked");
                locked.Add(s.SeatId);
            }

            var price = await _pricing.CalculatePrice();

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = req.UserId,
                ShowtimeId = req.ShowtimeId,
                Status = "PENDING",
                TotalPrice = price.total,
                DiscountAmount = price.discount,
                FinalPrice = price.final,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Version = 1
            };

            var seats = req.Seats.Select(s => new BookingSeat
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                SeatId = s.SeatId,
                SeatType = s.SeatType,
                TicketType = s.TicketType,
                Price = 100000,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            var promo = string.IsNullOrEmpty(req.PromotionCode)
                ? null
                : new BookingPromotion
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    PromotionCode = req.PromotionCode,
                    DiscountType = "PERCENT",
                    DiscountValue = 10
                };

            await _repo.CreateAsync(booking, seats, promo);
            await _seatClient.BookSeats(req.ShowtimeId, locked);

            return booking.Id;
        }
        catch
        {
            foreach (var seat in locked)
                await _seatClient.ReleaseSeat(
                    req.ShowtimeId, seat, req.UserId.ToString());
            throw;
        }
    }
}
