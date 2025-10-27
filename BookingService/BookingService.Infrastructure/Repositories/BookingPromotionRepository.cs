using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Infrastructure.Repositories
{
    public class BookingPromotionRepository : IBookingPromotionRepository
    {
        private readonly BookingDbContext _context;

        public BookingPromotionRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingPromotion>> GetAllAsync()
            => await _context.BookingPromotions.ToListAsync();

        public async Task<BookingPromotion?> GetByIdAsync(Guid id)
            => await _context.BookingPromotions.FindAsync(id);

        public async Task AddAsync(BookingPromotion bookingPromotion)
        {
            _context.BookingPromotions.Add(bookingPromotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookingPromotion bookingPromotion)
        {
            _context.BookingPromotions.Update(bookingPromotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.BookingPromotions.FindAsync(id);
            if (entity != null)
            {
                _context.BookingPromotions.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
