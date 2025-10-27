using ShowtimeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Domain.Interfaces
{
    public interface IShowtimeSeatRepository : IGenericRepository<ShowtimeSeat>
    {
        Task<IEnumerable<ShowtimeSeat>> GetByShowtimeIdAsync(Guid showtimeId);
        Task<IEnumerable<ShowtimeSeat>> GetAvailableSeatsAsync(Guid showtimeId);
        Task UpdateSeatStatusAsync(Guid seatId, string status);
    }
}
