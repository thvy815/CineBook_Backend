using BookingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Interfaces
{
    public interface IBookingPromotionRepository
    {
        Task<IEnumerable<BookingPromotion>> GetAllAsync();
        Task<BookingPromotion?> GetByIdAsync(Guid id);
        Task AddAsync(BookingPromotion bookingPromotion);
        Task UpdateAsync(BookingPromotion bookingPromotion);
        Task DeleteAsync(Guid id);
    }
}
