using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class ShowtimeSeatService
    {
        private readonly ShowtimeDbContext _context;
        public ShowtimeSeatService(ShowtimeDbContext context) => _context = context;

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
    }
}
