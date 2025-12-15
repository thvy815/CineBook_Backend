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
    public class ShowtimeRepository : GenericRepository<Showtime>, IShowtimeRepository
    {
        public ShowtimeRepository(ShowtimeDbContext context) : base(context) { }

        public async Task<IEnumerable<Showtime>> GetByTheaterIdAsync(Guid theaterId)
        {
            return await _context.Showtimes
                .Where(s => s.TheaterId == theaterId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetByMovieIdAsync(Guid movieId)
        {
            return await _context.Showtimes
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetByDateAsync(DateTime date)
        {
            return await _context.Showtimes
                .Where(s => s.StartTime.Date == date.Date)
                .ToListAsync();
        }
    }
}
