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
    private readonly ZaloPayClient _zaloPayClient;

    public BookingBusiness(
        ShowtimeSeatClient seatClient,
        PricingClient pricing,
        IBookingRepository repo,
        ZaloPayClient zaloPayClient)
    {
        _seatClient = seatClient;
        _pricing = pricing;
        _repo = repo;
        _zaloPayClient = zaloPayClient;
    }

    /// <summary>
    /// 1️⃣ Tạo booking PENDING + lock ghế
    /// </summary>
    public async Task<(Guid bookingId, List<Guid> lockedSeats)> CreateBookingPending(CreateBookingRequest req)
    {
        var lockedSeats = new List<Guid>();

        // 2️⃣ Gọi PricingService
        var pricingRequest = new PricingCalculateRequest
        {
            Seats = req.Seats
                .GroupBy(s => new { s.SeatType, s.TicketType })
                .Select(g => new PricingSeatDto
                {
                    SeatType = g.Key.SeatType,
                    TicketType = g.Key.TicketType,
                    Quantity = g.Sum(x => x.Quantity),
                }).ToList(),

            Fnbs = req.FnBs.Select(f => new PricingFnbDto
            {
                FnbItemId = f.FnbItemId,
                Name = "",        // tên để PricingService điền
                UnitPrice = 0,    // PricingService sẽ trả
                Quantity = f.Quantity
            }).ToList(),

            PromotionCode = req.PromotionCode
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

        var seats = req.Seats.Select((s, i) => new BookingSeat
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            SeatId = s.ShowtimeSeatId,
            SeatType = s.SeatType,
            TicketType = s.TicketType,
            Price = price.SeatPrice[i], // gán giá riêng cho từng ghế
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
        try
        {
            await _repo.CreateAsync(booking, seats, promo, fnbs);
        }
        catch (Exception ex)
        {
            Console.WriteLine("CreateAsync failed: " + ex.Message);
            throw;
        }

        return (booking.Id, lockedSeats);
    }

    // ==============================
    // 2️⃣ Tạo payment ZaloPay
    // ==============================
    public async Task<string> CreatePaymentForBooking(Guid bookingId)
    {
        var booking = await _repo.GetByIdAsync(bookingId);
        if (booking == null)
            throw new Exception("Booking not found");

        var (orderUrl, transactionId) =
            await _zaloPayClient.CreateOrderAsync(
                booking.Id,
                booking.UserId,
                booking.FinalPrice
            );

        await _repo.UpdateStatusAsync(
            booking.Id,
            "PENDING",
            transactionId,
            "ZaloPay"
        );

        return orderUrl;
    }

    // ==============================
    // 3️⃣ Callback từ PaymentService
    // ==============================
    public async Task HandlePaymentCallback(
        Guid bookingId,
        string status,          // SUCCESS / FAILED
        string transactionId,
        string paymentMethod)
    {
        var booking = await _repo.GetByIdAsync(bookingId);
        if (booking == null)
            throw new Exception("Booking not found");
        Console.WriteLine("Hi handle1");

        await _repo.UpdateStatusAsync(
            bookingId,
            status,
            transactionId,
            paymentMethod
        );

        var seatIds = booking.Seats.Select(s => s.SeatId).ToList();

        if (status == "SUCCESS")
        {
            Console.WriteLine("Hi handle if");
            try
            {
                if (status == "SUCCESS")
                {
                    await _seatClient.BookSeats(booking.ShowtimeId, seatIds);
                    await _repo.UpdateStatusAsync(booking.Id, "SUCCESS", transactionId, "ZaloPay");
                }
                else
                {
                    foreach (var seatId in seatIds)
                        await _seatClient.ReleaseSeat(booking.ShowtimeId, seatId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 HandlePaymentCallback failed: " + ex);
                throw;
            } 
        }
        else
        {
            foreach (var seatId in seatIds)
                await _seatClient.ReleaseSeat(booking.ShowtimeId, seatId);
        }
    }

    // ==============================
    // 4️⃣ Get booking
    // ==============================
    public async Task<Booking> GetBookingById(Guid bookingId)
    {
        return await _repo.GetByIdAsync(bookingId);
    }
}
