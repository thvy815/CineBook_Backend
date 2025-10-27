using BookingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Interfaces
{
    public interface IBookingFnbRepository
    {
        Task<IEnumerable<BookingFnb>> GetAllAsync();
        Task<BookingFnb?> GetByIdAsync(Guid id);
        Task AddAsync(BookingFnb bookingFnb);
        Task UpdateAsync(BookingFnb bookingFnb);
        Task DeleteAsync(Guid id);
    }
}
