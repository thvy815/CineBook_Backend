using MovieService.Domain.Entities;
using MovieService.Domain.Interfaces;

namespace MovieService.Application.Services
{
    public class MovieSummaryService
    {
        private readonly IMovieSummaryRepository _repo;

        public MovieSummaryService(IMovieSummaryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<MovieSummary>> GetAllAsync() => await _repo.GetAllAsync();
        public async Task<MovieSummary?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);
        public async Task AddAsync(MovieSummary movie) => await _repo.AddAsync(movie);
        public async Task UpdateAsync(MovieSummary movie) => await _repo.UpdateAsync(movie);
        public async Task DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
