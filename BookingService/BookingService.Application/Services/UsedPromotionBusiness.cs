using BookingService.Domain.DTOs;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;


namespace BookingService.Application.Services
{
    public class UsedPromotionBusiness
    {
        private readonly IUsedPromotionRepository _repository;

        public UsedPromotionBusiness(IUsedPromotionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UsedPromotionDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new UsedPromotionDTOs
            {
                Id = e.Id,
                UserId = e.UserId,
                PromotionCode = e.PromotionCode,
                BookingId = e.BookingId,
                UsedAt = e.UsedAt
            });
        }

        public async Task<UsedPromotionDTOs?> GetByIdAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new UsedPromotionDTOs
            {
                Id = e.Id,
                UserId = e.UserId,
                PromotionCode = e.PromotionCode,
                BookingId = e.BookingId,
                UsedAt = e.UsedAt
            };
        }

        public async Task AddAsync(UsedPromotionDTOs dto)
        {
            var entity = new UsedPromotion
            {
                Id = dto.Id,
                UserId = dto.UserId,
                PromotionCode = dto.PromotionCode,
                BookingId = dto.BookingId,
                UsedAt = dto.UsedAt
            };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(UsedPromotionDTOs dto)
        {
            var entity = new UsedPromotion
            {
                Id = dto.Id,
                UserId = dto.UserId,
                PromotionCode = dto.PromotionCode,
                BookingId = dto.BookingId,
                UsedAt = dto.UsedAt
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
