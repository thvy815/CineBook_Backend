using Microsoft.EntityFrameworkCore;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Interfaces;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using StackExchange.Redis;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeSeatService
    {
        private readonly ShowtimeDbContext _context;
        private readonly ISeatHubNotifier _notifier;
        private readonly IDatabase _redisDb;
        private readonly TimeSpan _lockTimeout = TimeSpan.FromMinutes(5); // ghế bị unlock sau 5 phút

        public ShowtimeSeatService(
            ShowtimeDbContext context,
            ISeatHubNotifier notifier,
            IConnectionMultiplexer redis)
        {
            _context = context;
            _notifier = notifier;
            _redisDb = redis.GetDatabase();
        }

        // Lấy danh sách ghế theo showtime
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
                    SeatType = s.Seat.Type,
                    UpdatedAt = s.UpdatedAt
                }).ToListAsync();
        }

        public async Task<int> CreateShowtimeSeatsAsync(Guid showtimeId, int seatCount)
        {
            if (seatCount <= 0)
                throw new Exception("Seat count must be greater than 0");

            // 1. Lấy showtime
            var showtime = await _context.Showtimes
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == showtimeId);

            if (showtime == null)
                throw new Exception("Showtime not found");

            // 2. Check đã tạo ShowtimeSeat chưa
            bool existed = await _context.ShowtimeSeats
                .AnyAsync(s => s.ShowtimeId == showtimeId);

            if (existed)
                throw new Exception("Showtime seats already created");

            var roomId = showtime.RoomId;

            // 3. Tạo seat mới theo số lượng nhập
            var newSeats = new List<Seat>();
            int seatsPerRow = 10; // số ghế tối đa mỗi row
            int rowIndex = 0;
            for (int i = 0; i < seatCount; i++)
            {
                if (i % seatsPerRow == 0 && i != 0) rowIndex++;

                char rowLabelChar = (char)('A' + rowIndex);
                string rowLabel = rowLabelChar.ToString();
                int columnIndex = (i % seatsPerRow) + 1;

                var seat = new Seat
                {
                    Id = Guid.NewGuid(),
                    RoomId = roomId,
                    RowLabel = rowLabel,
                    ColumnIndex = columnIndex,
                    SeatNumber = $"{rowLabel}{columnIndex}",
                    Type = "Normal" // có thể truyền thêm nếu muốn
                };

                newSeats.Add(seat);
            }

            await _context.Seats.AddRangeAsync(newSeats);

            // 4. Tạo ShowtimeSeat tương ứng
            var showtimeSeats = newSeats.Select(s => new ShowtimeSeat
            {
                Id = Guid.NewGuid(),
                ShowtimeId = showtimeId,
                SeatId = s.Id,
                Status = "Available",
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            await _context.ShowtimeSeats.AddRangeAsync(showtimeSeats);
            await _context.SaveChangesAsync();

            return showtimeSeats.Count;
        }



        // Lock ghế
        public async Task<bool> TryLockSeatDb(Guid showtimeId, Guid seatId)
        {
            // 1️⃣ Dùng atomic DB update để chống race condition
            var updated = await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Available")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Blocked")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                );

            if (updated == 0)
                return false; // Ghế đã bị lock hoặc booked

            // 2️⃣ Set Redis key để auto unlock
            string redisKey = $"seat:{showtimeId}:{seatId}";
            try
            {
                bool redisSet = await _redisDb.StringSetAsync(redisKey, "locked", _lockTimeout, when: StackExchange.Redis.When.NotExists);
                if (!redisSet)
                {
                    // Redis đã có key → rollback DB
                    await _context.ShowtimeSeats
                        .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Blocked")
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "Available"));

                    return false;
                }
            }
            catch
            {
                // Nếu Redis throw lỗi, rollback DB
                await _context.ShowtimeSeats
                    .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Blocked")
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, "Available"));

                throw; // Rethrow exception để tầng trên biết
            }

            // 3️⃣ Notify client qua SignalR
            await _notifier.NotifySeatUpdated(showtimeId, seatId.ToString(), true);

            return true;
        }

        public async Task<bool> ReleaseSeatDb(Guid showtimeId, Guid seatId)
        {
            // Atomic update: chỉ release nếu ghế đang Blocked
            var updated = await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Blocked")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Available")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                );

            if (updated == 0)
                return false; // Ghế không bị blocked hoặc đã booked

            // Xóa Redis key nếu còn
            string redisKey = $"seat:{showtimeId}:{seatId}";
            await _redisDb.KeyDeleteAsync(redisKey);

            // Notify client qua SignalR
            await _notifier.NotifySeatUpdated(showtimeId, seatId.ToString(), false);

            return true;
        }

        public async Task<bool> BookSeatDb(Guid showtimeId, Guid seatId)
        {
            // Atomic update: chỉ book nếu ghế đang Blocked
            var updated = await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Blocked")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Booked")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                );

            if (updated == 0)
                return false; // Ghế không bị blocked, có thể đã release hoặc book rồi

            // Xóa Redis key nếu còn
            string redisKey = $"seat:{showtimeId}:{seatId}";
            await _redisDb.KeyDeleteAsync(redisKey);

            // Notify client qua SignalR
            await _notifier.NotifySeatUpdated(showtimeId, seatId.ToString(), false);

            return true;
        }
        public async Task HandleSeatTimeout(string redisKey)
        {
            // redisKey format: seat:{showtimeId}:{seatId}
            var parts = redisKey.Split(':');
            if (parts.Length != 3) return;

            if (!Guid.TryParse(parts[1], out Guid showtimeId)) return;
            if (!Guid.TryParse(parts[2], out Guid seatId)) return;

            // Atomic update: chỉ release nếu ghế vẫn Blocked
            var updated = await _context.ShowtimeSeats
                .Where(s => s.ShowtimeId == showtimeId && s.Id == seatId && s.Status == "Blocked")
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.Status, "Available")
                    .SetProperty(x => x.UpdatedAt, DateTime.UtcNow)
                );

            if (updated == 0) return; // Ghế đã bị book hoặc release rồi

            // Notify client để UI cập nhật ghế
            await _notifier.NotifySeatUpdated(showtimeId, seatId.ToString(), false);
        }


    }
}
