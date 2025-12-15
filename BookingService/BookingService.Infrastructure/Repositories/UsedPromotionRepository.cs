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
//    public class UsedPromotionRepository : IUsedPromotionRepository
//    {
//        private readonly BookingDbContext _context;

//        public UsedPromotionRepository(BookingDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<UsedPromotion>> GetAllAsync()
//            => await _context.UsedPromotions.ToListAsync();

//        public async Task<UsedPromotion?> GetByIdAsync(Guid id)
//            => await _context.UsedPromotions.FindAsync(id);

//        public async Task AddAsync(UsedPromotion usedPromotion)
//        {
//            _context.UsedPromotions.Add(usedPromotion);
//            await _context.SaveChangesAsync();
//        }

//        public async Task UpdateAsync(UsedPromotion usedPromotion)
//        {
//            _context.UsedPromotions.Update(usedPromotion);
//            await _context.SaveChangesAsync();
//        }

//        public async Task DeleteAsync(Guid id)
//        {
//            var entity = await _context.UsedPromotions.FindAsync(id);
//            if (entity != null)
//            {
//                _context.UsedPromotions.Remove(entity);
//                await _context.SaveChangesAsync();
//            }
//        }
//    }
//}
