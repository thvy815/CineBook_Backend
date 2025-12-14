using BookingService.Application.Clients;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BookingService.Application.Services
{
    /// <summary>
    /// Background service để expire các booking PENDING quá thời gian
    /// </summary>
    public class BookingExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10); // mỗi 10s check 1 lần
        private readonly TimeSpan _pendingTimeout = TimeSpan.FromMinutes(5); // ví dụ: 5 phút lock

        public BookingExpirationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var bookingRepo = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                        var seatClient = scope.ServiceProvider.GetRequiredService<ShowtimeSeatClient>();

                        // 1️⃣ Lấy tất cả booking PENDING
                        var pendingBookings = await bookingRepo.GetPendingBookingsAsync();

                        foreach (var booking in pendingBookings)
                        {
                            // Nếu booking đã quá thời gian PENDING
                            if (DateTime.UtcNow - booking.CreatedAt > _pendingTimeout)
                            {
                                // 2️⃣ Release tất cả ghế đã lock
                                var lockedSeatIds = booking.Seats.Select(s => s.SeatId).ToList();
                                foreach (var seatId in lockedSeatIds)
                                {
                                    await seatClient.ReleaseSeat(booking.ShowtimeId, seatId);
                                }

                                // 3️⃣ Update booking status = FAIL
                                await bookingRepo.UpdateStatusAsync(
                                    booking.Id,
                                    "FAIL",
                                    transactionId: "",
                                    paymentMethod: ""
                                );
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // log lỗi nếu có
                    Console.WriteLine($"[BookingExpirationService] Error: {ex.Message}");
                }

                // Delay trước khi check lần tiếp theo
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
