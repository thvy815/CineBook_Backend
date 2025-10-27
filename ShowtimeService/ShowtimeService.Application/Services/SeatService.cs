using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ShowtimeService.Application.Services
{
    public class SeatService
    {
        private readonly ShowtimeDbContext _context;
        public SeatService(ShowtimeDbContext context) => _context = context;

        public async Task<IEnumerable<SeatDto>> GetByRoomAsync(Guid roomId)
        {
            return await _context.Seats
                .Where(s => s.RoomId == roomId)
                .Select(s => new SeatDto
                {
                    Id = s.Id,
                    RoomId = s.RoomId,
                    SeatNumber = s.SeatNumber,
                    RowLabel = s.RowLabel,
                    ColumnIndex = s.ColumnIndex,
                    Type = s.Type
                }).ToListAsync();
        }

        public async Task<SeatDto> CreateAsync(CreateSeatDto dto)
        {
            var entity = new Seat
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                SeatNumber = dto.SeatNumber,
                RowLabel = dto.RowLabel,
                ColumnIndex = dto.ColumnIndex,
                Type = dto.Type
            };
            _context.Seats.Add(entity);
            await _context.SaveChangesAsync();
            return new SeatDto
            {
                Id = entity.Id,
                RoomId = entity.RoomId,
                SeatNumber = entity.SeatNumber,
                RowLabel = entity.RowLabel,
                ColumnIndex = entity.ColumnIndex,
                Type = entity.Type
            };
        }
    }
}
