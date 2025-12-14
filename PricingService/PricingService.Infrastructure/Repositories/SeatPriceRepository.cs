using PricingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PricingService.Infrastructure.Repositories
{
    public class SeatPriceRepository : ISeatPriceRepository
    {
        private readonly PricingDbContext _context;

        public SeatPriceRepository(PricingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeatPrice>> GetAllAsync() => await _context.SeatPrices.ToListAsync();

        public async Task<SeatPrice?> GetByIdAsync(Guid id) => await _context.SeatPrices.FindAsync(id);

        public async Task AddAsync(SeatPrice seatPrice)
        {
            _context.SeatPrices.Add(seatPrice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SeatPrice seatPrice)
        {
            _context.SeatPrices.Update(seatPrice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var seatPrice = await _context.SeatPrices.FindAsync(id);
            if (seatPrice != null)
            {
                _context.SeatPrices.Remove(seatPrice);
                await _context.SaveChangesAsync();
            }
        }
    }
}
