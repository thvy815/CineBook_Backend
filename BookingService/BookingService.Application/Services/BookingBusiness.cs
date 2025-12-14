using BookingService.Application.Clients;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using System.Net;

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

    /// <summary>
    /// 1️⃣ Tạo booking PENDING + lock ghế
    /// </summary>
    public async Task<(Guid bookingId, List<Guid> lockedSeats)> CreateBookingPending(CreateBookingRequest req)
    {
        var lockedSeats = new List<Guid>();

        // Lock tất cả ghế trước
        foreach (var s in req.Seats)
        {
            var ok = await _seatClient.LockSeat(req.ShowtimeId, s.ShowtimeSeatId);
            if (!ok)
            {
                // Rollback tất cả ghế đã lock
                foreach (var locked in lockedSeats)
                    await _seatClient.ReleaseSeat(req.ShowtimeId, locked);

                throw new Exception($"Seat {s.ShowtimeSeatId} is already booked or blocked.");
            }
            lockedSeats.Add(s.ShowtimeSeatId);
        }

        // 2️⃣ Gọi PricingService
        var pricingRequest = new PricingCalculateRequest
        {
            Seats = req.Seats
                .GroupBy(s => new { s.SeatType, s.TicketType })
                .Select(g => new PricingSeatDto
                {
                    SeatType = g.Key.SeatType,
                    TicketType = g.Key.TicketType,
                }).ToList(),

            Fnbs = req.FnBs.Select(f => new PricingFnbDto
            {
                FnbItemId = f.FnbItemId,
                Name = "",        // tên để PricingService điền
                UnitPrice = 0,    // PricingService sẽ trả
                Quantity = f.Quantity
            }).ToList(),
        };
        
        var price = await _pricing.CalculateAsync(pricingRequest);

        // 3️⃣ Tạo booking và lưu giá
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = req.UserId,
            ShowtimeId = req.ShowtimeId,
            Status = "PENDING",

            TotalPrice = price.SubTotal,
            DiscountAmount = price.Discount,
            FinalPrice = price.FinalTotal,

            TransactionId = "",
            PaymentMethod = "",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Version = 1
        };

        // 4️⃣ Lưu booking seat
        var seats = req.Seats.Select(s => new BookingSeat
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            SeatId = s.ShowtimeSeatId,
            SeatType = s.SeatType,
            TicketType = s.TicketType,
            Price = price.SeatTotal / req.Seats.Count, // gán giá trung bình cho mỗi ghế
            CreatedAt = DateTime.UtcNow
        }).ToList();

        // Lưu FnB với đơn giá từ PricingService
        var fnbs = price.Fnbs.Select(f => new BookingFnb
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            FnbItemId = f.FnbItemId,
            Quantity = f.Quantity,
            UnitPrice = f.UnitPrice,
            TotalFnbPrice = f.UnitPrice * f.Quantity
        }).ToList();

        // Lưu promo
        BookingPromotion? promo = null;
        if (price.Promotion != null)
        {
            promo = new BookingPromotion
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                PromotionCode = price.Promotion.Code,
                DiscountType = price.Promotion.DiscountType,
                DiscountValue = price.Promotion.DiscountValue
            };
        }

        // 7️⃣ Lưu vào repository
        await _repo.CreateAsync(booking, seats, promo, fnbs);

        return (booking.Id, lockedSeats);
    }

    /// <summary>
    /// 2️⃣ Cập nhật booking khi thanh toán ZaloPay callback
    /// </summary>
    public async Task HandlePaymentCallback(
        Guid bookingId,
        string status,            // "SUCCESSFUL" hoặc "FAIL"
        string transactionId,
        string paymentMethod,
        Guid showtimeId,
        List<Guid> seatIds,
        string userId)
    {
        // 1. Cập nhật status, transactionId, paymentMethod
        await _repo.UpdateStatusAsync(bookingId, status, transactionId, paymentMethod);

        // 2. Nếu thành công → book ghế thực sự
        if (status == "SUCCESSFUL")
        {
            await _seatClient.BookSeats(showtimeId, seatIds);
        }
        else
        {
            foreach (var seatId in seatIds)
                await _seatClient.ReleaseSeat(showtimeId, seatId);
        }
    }

    /// <summary>
    /// 3️⃣ Lấy booking info (nếu cần)
    /// </summary>
    public async Task<Booking> GetBookingById(Guid bookingId)
    {
        return await _repo.GetByIdAsync(bookingId);
    }
}
