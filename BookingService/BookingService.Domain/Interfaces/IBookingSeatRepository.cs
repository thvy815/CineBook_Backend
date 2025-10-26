using BookingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Interfaces
{
    public interface IBookingSeatRepository
    {
        Task<IEnumerable<BookingSeat>> GetAllAsync();
        Task<BookingSeat?> GetByIdAsync(Guid id);
        Task AddAsync(BookingSeat bookingSeat);
        Task UpdateAsync(BookingSeat bookingSeat);
        Task DeleteAsync(Guid id);
    }
}
