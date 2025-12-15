using ShowtimeService.Application.DTOs;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace ShowtimeService.Application.Services
{
    public class SeatService
    {
        private readonly ShowtimeDbContext _context;
    

        public SeatService(ShowtimeDbContext context)
        {
            _context = context;
        }

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

        public async Task<List<SeatDto>> GenerateSeatsAsync(CreateSeatsRequest request)
        {
            var seats = new List<Seat>();
            var rowLabels = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            for (int r = 0; r < request.Rows; r++)
            {
                int colIndex = 1;

                while (colIndex <= request.Columns)
                {
                    string type = "SINGLE";

                    // Ghế đôi
                    if (request.DoubleSeats > 0 && colIndex < request.Columns)
                    {
                        type = "DOUBLE";
                        request.DoubleSeats--;
                    }

                    seats.Add(new Seat
                    {
                        Id = Guid.NewGuid(),
                        RoomId = request.RoomId,
                        RowLabel = rowLabels[r].ToString(),
                        ColumnIndex = colIndex,
                        SeatNumber = $"{rowLabels[r]}{colIndex}",
                        Type = type
                    });

                    colIndex += type == "DOUBLE" ? 2 : 1;
                }
            }

            // LƯU DB
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();

            // Trả về dạng DTO
            return seats.Select(s => new SeatDto
            {
                Id = s.Id,
                RoomId = s.RoomId,
                RowLabel = s.RowLabel,
                ColumnIndex = s.ColumnIndex,
                SeatNumber = s.SeatNumber,
                Type = s.Type
            }).ToList();
        }

    }
}
