using Microsoft.EntityFrameworkCore;
using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;

namespace MovieService.Infrastructure.Repositories
{
    public class MovieDetailRepository : IMovieDetailRepository
    {
        private readonly MovieDbContext _db;

        public MovieDetailRepository(MovieDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<MovieDetail>> GetAllAsync()
            => await _db.MovieDetails.ToListAsync();

        public async Task<MovieDetail?> GetByIdAsync(Guid id)
            => await _db.MovieDetails.FindAsync(id);

        public async Task AddAsync(MovieDetail movie)
        {
            _db.MovieDetails.Add(movie);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(MovieDetail movie)
        {
            _db.MovieDetails.Update(movie);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.MovieDetails.FindAsync(id);
            if (entity != null)
            {
                _db.MovieDetails.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
