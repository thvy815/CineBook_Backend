using ShowtimeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Domain.Interfaces
{
    public interface IShowtimeRepository : IGenericRepository<Showtime>
    {
        Task<IEnumerable<Showtime>> GetByTheaterIdAsync(Guid theaterId);
        Task<IEnumerable<Showtime>> GetByMovieIdAsync(Guid movieId);
        Task<IEnumerable<Showtime>> GetByDateAsync(DateTime date);
    }
}
