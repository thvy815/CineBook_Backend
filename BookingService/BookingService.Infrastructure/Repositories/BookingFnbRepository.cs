//using BookingService.Domain.Entities;
//using BookingService.Domain.Interfaces;
//using BookingService.Infrastructure.Data;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BookingService.Infrastructure.Repositories
//{
//    public class BookingFnbRepository : IBookingFnbRepository
//    {
//        private readonly BookingDbContext _context;

//        public BookingFnbRepository(BookingDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<BookingFnb>> GetAllAsync()
//            => await _context.BookingFnbs.ToListAsync();

//        public async Task<BookingFnb?> GetByIdAsync(Guid id)
//            => await _context.BookingFnbs.FindAsync(id);

//        public async Task AddAsync(BookingFnb bookingFnb)
//        {
//            _context.BookingFnbs.Add(bookingFnb);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(BookingFnb bookingFnb)
//        {
//            _context.BookingFnbs.Update(bookingFnb);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(Guid id)
//        {
//            var entity = await _context.BookingFnbs.FindAsync(id);
//            if (entity != null)
//            {
//                _context.BookingFnbs.Remove(entity);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
