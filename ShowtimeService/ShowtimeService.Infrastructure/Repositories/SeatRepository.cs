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
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<IEnumerable<Seat>> GetByRoomIdAsync(Guid roomId)
        {
            return await _context.Seats
                .Where(s => s.RoomId == roomId)
                .ToListAsync();
        }
    }
}
