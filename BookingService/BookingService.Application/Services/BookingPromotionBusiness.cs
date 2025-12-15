using BookingService.Domain.DTOs;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;


namespace BookingService.Application.Services
{
    public class BookingPromotionBusiness
    {
        private readonly IBookingPromotionRepository _repository;

        public BookingPromotionBusiness(IBookingPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookingPromotionDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new BookingPromotionDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                PromotionCode = e.PromotionCode,
                DiscountType = e.DiscountType,
                DiscountValue = e.DiscountValue
            });
        }

        public async Task<BookingPromotionDTOs?> GetByIdAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new BookingPromotionDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                PromotionCode = e.PromotionCode,
                DiscountType = e.DiscountType,
                DiscountValue = e.DiscountValue
            };
        }

        public async Task AddAsync(BookingPromotionDTOs dto)
        {
            var entity = new BookingPromotion
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                PromotionCode = dto.PromotionCode,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue
            };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(BookingPromotionDTOs dto)
        {
            var entity = new BookingPromotion
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                PromotionCode = dto.PromotionCode,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
