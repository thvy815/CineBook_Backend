using Microsoft.EntityFrameworkCore;
using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using ShowtimeService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowtimeService.Infrastructure.Repositories
{
    public class ShowtimeSeatRepository : GenericRepository<ShowtimeSeat>, IShowtimeSeatRepository
    {
        public ShowtimeSeatRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<IEnumerable<ShowtimeSeat>> GetByShowtimeIdAsync(Guid showtimeId)
        {
            return await _context.ShowtimeSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowtimeId == showtimeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ShowtimeSeat>> GetAvailableSeatsAsync(Guid showtimeId)
        {
            return await _context.ShowtimeSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowtimeId == showtimeId && ss.Status == "Available")
                .ToListAsync();
        }

        public async Task UpdateSeatStatusAsync(Guid seatId, string status)
        {
            var seat = await _context.ShowtimeSeats.FirstOrDefaultAsync(s => s.SeatId == seatId);
            if (seat != null)
            {
                seat.Status = status;
                seat.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
