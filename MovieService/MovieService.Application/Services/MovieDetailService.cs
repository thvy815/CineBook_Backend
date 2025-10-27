using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;

namespace MovieService.Application.Services
{
    public class MovieDetailService
    {
        private readonly IMovieDetailRepository _repo;

        public MovieDetailService(IMovieDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MovieDetail>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<MovieDetail?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(MovieDetail movie) => await _repo.AddAsync(movie);
        public async Task UpdateAsync(MovieDetail movie) => await _repo.UpdateAsync(movie);
        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
