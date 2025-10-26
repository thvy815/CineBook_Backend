using ShowtimeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Domain.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<IEnumerable<Seat>> GetByRoomIdAsync(Guid roomId);
    }
}
