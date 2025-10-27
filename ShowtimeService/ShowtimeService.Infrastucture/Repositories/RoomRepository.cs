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
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<IEnumerable<Room>> GetByTheaterIdAsync(Guid theaterId)
        {
            return await _context.Rooms
                .Where(r => r.TheaterId == theaterId)
                .ToListAsync();
        }
    }
}
