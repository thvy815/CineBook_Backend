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
    public class TheaterRepository : GenericRepository<Theater>, ITheaterRepository
    {
        public TheaterRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<IEnumerable<Theater>> GetByProvinceIdAsync(Guid provinceId)
        {
            return await _context.Theaters
                .Where(t => t.ProvinceId == provinceId)
                .ToListAsync();
        }
    }
}
