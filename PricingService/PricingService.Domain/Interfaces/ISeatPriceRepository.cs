using PricingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Domain.Interfaces
{
    public interface ISeatPriceRepository
    {
        Task<IEnumerable<SeatPrice>> GetAllAsync();
        Task<SeatPrice?> GetByIdAsync(Guid id);
        Task AddAsync(SeatPrice seatPrice);
        Task UpdateAsync(SeatPrice seatPrice);
        Task DeleteAsync(Guid id);
    }
}
