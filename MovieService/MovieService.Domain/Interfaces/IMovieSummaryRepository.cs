using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces
{
    public interface IMovieSummaryRepository
    {
        Task<IEnumerable<MovieSummary>> GetAllAsync();
        Task<MovieSummary?> GetByIdAsync(int id);
        Task AddAsync(MovieSummary movie);
        Task UpdateAsync(MovieSummary movie);
        Task DeleteAsync(int id);
    }
}
