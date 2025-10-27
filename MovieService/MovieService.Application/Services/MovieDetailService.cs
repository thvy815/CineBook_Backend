using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace MovieService.Application.Services
{
    public class MovieDetailService
    {
        private readonly MovieDbContext _db;

        public MovieDetailService(MovieDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<MovieDetail>> GetAllAsync()
            => await _db.MovieDetails.ToListAsync();

        public async Task<MovieDetail?> GetByIdAsync(int id)
            => await _db.MovieDetails.FindAsync(id);

        public async Task AddAsync(MovieDetail movie)
        {
            _db.MovieDetails.Add(movie);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(MovieDetail movie)
        {
            var exists = await _db.MovieDetails.AnyAsync(m => m.Id == movie.Id);
            if (!exists) return false;

            _db.MovieDetails.Update(movie);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _db.MovieDetails.FindAsync(id);
            if (movie == null) return false;

            _db.MovieDetails.Remove(movie);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MovieDetail>> SearchAsync(string keyword)
            => await _db.MovieDetails
                .Where(m => m.Title.ToLower().Contains(keyword.ToLower()) ||
                            m.Genres.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();

        public async Task<IEnumerable<MovieDetail>> GetByStatusAsync(string status)
            => await _db.MovieDetails
                .Where(m => m.Status.ToLower() == status.ToLower())
                .ToListAsync();
    }

}
