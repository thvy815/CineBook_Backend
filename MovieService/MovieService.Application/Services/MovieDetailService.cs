using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;
using MovieService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using MovieService.Domain.DTOs;

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

        public async Task<MovieDetail?> GetByIdAsync(Guid id)
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

        public async Task<bool> DeleteAsync(Guid id)
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

        public async Task<PagedResponse<MovieDetail>> AdminSearchAsync(
        string? keyword,
        string? status,
        string? genres, // comma-separated
        int page = 1,
        int size = 10,
        string? sortBy = null,
        string? sortType = null)
        {
            var query = _db.MovieDetails.AsQueryable();

            // keyword search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();
                query = query.Where(m => m.Title.ToLower().Contains(kw));
            }

            // status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim().ToLower();
                query = query.Where(m => m.Status.ToLower() == s);
            }

            // genres filter
            if (!string.IsNullOrWhiteSpace(genres))
            {
                var genreList = genres.Split(", ").Select(g => g.Trim().ToLower()).ToList();
                query = query.Where(m => !string.IsNullOrEmpty(m.Genres) &&
                         genreList.Any(g => EF.Functions.ILike(m.Genres, $"%{g}%")));
            }

            // sorting
            var allowedSort = new List<string> { "CreatedAt", "Title", "Status" };
            var sortField = allowedSort.Contains(sortBy ?? "") ? sortBy : "CreatedAt";
            var ascending = sortType != null && sortType.Equals("ASC", StringComparison.OrdinalIgnoreCase);

            query = sortField switch
            {
                "Title" => ascending ? query.OrderBy(m => m.Title) : query.OrderByDescending(m => m.Title),
                "Status" => ascending ? query.OrderBy(m => m.Status) : query.OrderByDescending(m => m.Status),
                _ => query.OrderBy(m => m.Title) // default
            };

            // paging
            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)total / size);
            var skip = (page - 1) * size;

            var list = await query.Skip(skip).Take(size).ToListAsync();

            return new PagedResponse<MovieDetail>
            {
                Data = list,
                Page = page,
                Size = size,
                TotalElements = total,
                TotalPages = totalPages
            };
        }
    }
}
