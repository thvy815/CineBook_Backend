using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class RoomService
    {
        private readonly ShowtimeDbContext _context;
        public RoomService(ShowtimeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomDto>> GetByTheaterIdAsync(Guid theaterId)
        {
            return await _context.Rooms
                .Where(r => r.TheaterId == theaterId)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    TheaterId = r.TheaterId,
                    Name = r.Name,
                    SeatCount = r.SeatCount,
                    Status = r.Status
                }).ToListAsync();
        }

        public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
        {
            var entity = new Room
            {
                Id = Guid.NewGuid(),
                TheaterId = dto.TheaterId,
                Name = dto.Name,
                SeatCount = dto.SeatCount,
                Status = dto.Status
            };
            _context.Rooms.Add(entity);
            await _context.SaveChangesAsync();
            return new RoomDto
            {
                Id = entity.Id,
                TheaterId = entity.TheaterId,
                Name = entity.Name,
                SeatCount = entity.SeatCount,
                Status = entity.Status
            };
        }
    }
}
