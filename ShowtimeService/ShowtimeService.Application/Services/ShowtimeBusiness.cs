using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeBusiness
    {
        private readonly ShowtimeDbContext _context;
        public ShowtimeBusiness(ShowtimeDbContext context) => _context = context;

        // Lấy tất cả suất chiếu
        public async Task<IEnumerable<ShowtimeDto>> GetAllAsync()
        {
            return await _context.Showtimes
                .Select(s => new ShowtimeDto
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    TheaterId = s.TheaterId,
                    RoomId = s.RoomId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
                .ToListAsync();
        }

        // Lấy suất chiếu theo ID
        public async Task<ShowtimeDto?> GetByIdAsync(Guid id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null) return null;

            return new ShowtimeDto
            {
                Id = showtime.Id,
                MovieId = showtime.MovieId,
                TheaterId = showtime.TheaterId,
                RoomId = showtime.RoomId,
                StartTime = showtime.StartTime,
                EndTime = showtime.EndTime
            };
        }

        // Tạo mới suất chiếu
        public async Task<ShowtimeDto> CreateAsync(CreateShowtimeDto dto)
        {
            var entity = new Showtime
            {
                Id = Guid.NewGuid(),
                MovieId = dto.MovieId,
                TheaterId = dto.TheaterId,
                RoomId = dto.RoomId,
                StartTime = DateTime.SpecifyKind(dto.StartTime, DateTimeKind.Utc),
                EndTime = DateTime.SpecifyKind(dto.EndTime, DateTimeKind.Utc)
            };

            _context.Showtimes.Add(entity);
            await _context.SaveChangesAsync();

            return new ShowtimeDto
            {
                Id = entity.Id,
                MovieId = entity.MovieId,
                TheaterId = entity.TheaterId,
                RoomId = entity.RoomId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        // Cập nhật suất chiếu
        public async Task<ShowtimeDto?> UpdateAsync(Guid id, CreateShowtimeDto dto)
        {
            var entity = await _context.Showtimes.FindAsync(id);
            if (entity == null) return null;

            entity.MovieId = dto.MovieId;
            entity.TheaterId = dto.TheaterId;
            entity.RoomId = dto.RoomId;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;

            await _context.SaveChangesAsync();

            return new ShowtimeDto
            {
                Id = entity.Id,
                MovieId = entity.MovieId,
                TheaterId = entity.TheaterId,
                RoomId = entity.RoomId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        // Xóa suất chiếu
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.Showtimes.FindAsync(id);
            if (entity == null) return false;

            _context.Showtimes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
