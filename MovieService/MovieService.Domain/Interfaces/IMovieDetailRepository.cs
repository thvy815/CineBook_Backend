using MovieService.Domain.Entities;

namespace MovieService.Domain.Interfaces
{
    public interface IMovieDetailRepository
    {
        Task<IEnumerable<MovieDetail>> GetAllAsync();
        Task<MovieDetail?> GetByIdAsync(int id);
        Task AddAsync(MovieDetail movie);
        Task UpdateAsync(MovieDetail movie);
        Task DeleteAsync(int id);
    }
}
