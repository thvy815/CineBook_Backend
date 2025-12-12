using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeSeatService
    {
        private readonly ShowtimeDbContext _context;
        private readonly IDatabase _db;
        public ShowtimeSeatService(ShowtimeDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _db = redis.GetDatabase(); // Đây là bước thiếu
        }

            public async Task<IEnumerable<ShowtimeSeatDto>> GetByShowtimeAsync(Guid showtimeId)
        {
            return await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId)
                .Select(s => new ShowtimeSeatDto
                {
                    Id = s.Id,
                    ShowtimeId = s.ShowtimeId,
                    SeatId = s.SeatId,
                    Status = s.Status,
                    UpdatedAt = s.UpdatedAt
                }).ToListAsync();
        }

        public async Task<ShowtimeSeatDto> CreateAsync(CreateShowtimeSeatDto dto)
        {
            var entity = new ShowtimeSeat
            {
                Id = Guid.NewGuid(),
                ShowtimeId = dto.ShowtimeId,
                SeatId = dto.SeatId,
                Status = dto.Status,
                UpdatedAt = DateTime.SpecifyKind(dto.UpdatedAt, DateTimeKind.Utc)
            };
            _context.ShowtimeSeats.Add(entity);
            await _context.SaveChangesAsync();
            return new ShowtimeSeatDto
            {
                Id = entity.Id,
                ShowtimeId = entity.ShowtimeId,
                SeatId = entity.SeatId,
                Status = entity.Status,
                UpdatedAt = entity.UpdatedAt
            };
        }

        // 🔹 Tạo tất cả ghế cho một suất chiếu theo room
        public async Task<IEnumerable<ShowtimeSeatDto>> CreateSeatsForShowtimeAsync(Guid showtimeId, Guid roomId)
        {
            // Lấy tất cả ghế của room
            var seats = await _context.Seats
                .Where(s => s.RoomId == roomId)
                .ToListAsync();

            var showtimeSeats = seats.Select(s => new ShowtimeSeat
            {
                Id = Guid.NewGuid(),
                ShowtimeId = showtimeId,
                SeatId = s.Id,
                Status = "Available",
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            _context.ShowtimeSeats.AddRange(showtimeSeats);
            await _context.SaveChangesAsync();

            return showtimeSeats.Select(s => new ShowtimeSeatDto
            {
                Id = s.Id,
                ShowtimeId = s.ShowtimeId,
                SeatId = s.SeatId,
                Status = s.Status,
                UpdatedAt = s.UpdatedAt
            });
        }

        public async Task<bool> TryLockSeatDb(Guid showtimeId, Guid showtimeSeatId, string userId)
        {
            var seat = await _context.ShowtimeSeats
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.Id == showtimeSeatId);

            // Nếu đã có và đang Available → block
            if (seat.Status == "Available")
            {
                seat.Status = "Blocked";
                seat.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            // Nếu đang Blocked/Booked → không cho block
            return false;
        }

        public async Task<bool> ReleaseSeatDb(Guid showtimeId, Guid showtimeSeatId, string userId)
        {
            var seat = await _context.ShowtimeSeats
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.Id == showtimeSeatId);

            if (seat != null && seat.Status == "Blocked")
            {
                seat.Status = "Available";
                seat.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<List<Guid>> GetLockedSeatsDb(Guid showtimeId)
        {
            return await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId && s.Status == "Blocked")
                .Select(s => s.SeatId)
                .ToListAsync();
        }

        public async Task<bool> BookSeatDb(Guid showtimeId, Guid showtimeSeatId)
        {
            var seat = await _context.ShowtimeSeats
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId && s.Id == showtimeSeatId);

            if (seat == null) return false;

            seat.Status = "Booked";
            seat.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
