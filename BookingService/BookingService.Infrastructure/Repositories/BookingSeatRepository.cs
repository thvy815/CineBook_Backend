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
    public class BookingSeatRepository : IBookingSeatRepository
    {
        private readonly BookingDbContext _context;

        public BookingSeatRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingSeat>> GetAllAsync()
            => await _context.BookingSeats.ToListAsync();

        public async Task<BookingSeat?> GetByIdAsync(Guid id)
            => await _context.BookingSeats.FindAsync(id);

        public async Task AddAsync(BookingSeat bookingSeat)
        {
            _context.BookingSeats.Add(bookingSeat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookingSeat bookingSeat)
        {
            _context.BookingSeats.Update(bookingSeat);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.BookingSeats.FindAsync(id);
            if (entity != null)
            {
                _context.BookingSeats.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
