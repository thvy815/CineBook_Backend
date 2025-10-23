using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;

namespace MovieService.Infrastructure.Repositories
{
    public class MovieSummaryRepository : IMovieSummaryRepository
    {
        private readonly MovieDbContext _db;

        public MovieSummaryRepository(MovieDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<MovieSummary>> GetAllAsync()
            => await _db.MovieSummaries.ToListAsync();

        public async Task<MovieSummary?> GetByIdAsync(int id)
            => await _db.MovieSummaries.FindAsync(id);

        public async Task AddAsync(MovieSummary movie)
        {
            _db.MovieSummaries.Add(movie);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovieSummary movie)
        {
            _db.MovieSummaries.Update(movie);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.MovieSummaries.FindAsync(id);
            if (entity != null)
            {
                _db.MovieSummaries.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
