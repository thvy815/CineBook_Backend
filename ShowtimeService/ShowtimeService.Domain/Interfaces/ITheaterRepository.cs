using ShowtimeService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Domain.Interfaces
{
    public interface ITheaterRepository : IGenericRepository<Theater>
    {
        Task<IEnumerable<Theater>> GetByProvinceIdAsync(Guid provinceId);
    }
}
