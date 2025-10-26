using BookingService.Domain.DTOs;
using BookingService.Domain.Entities;
using BookingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Application.Services
{
    public class BookingBusiness
    {
        private readonly IBookingRepository _repository;

        public BookingBusiness(IBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookingDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new BookingDTOs
            {
                Id = e.Id,
                UserId = e.UserId,
                ShowtimeId = e.ShowtimeId,
                Status = e.Status,
                PaymentMethod = e.PaymentMethod,
                TransactionId = e.TransactionId,
                TotalPrice = e.TotalPrice,
                DiscountAmount = e.DiscountAmount,
                FinalPrice = e.FinalPrice,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Version = e.Version
            });
        }

        public async Task<BookingDTOs?> GetByIdAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new BookingDTOs
            {
                Id = e.Id,
                UserId = e.UserId,
                ShowtimeId = e.ShowtimeId,
                Status = e.Status,
                PaymentMethod = e.PaymentMethod,
                TransactionId = e.TransactionId,
                TotalPrice = e.TotalPrice,
                DiscountAmount = e.DiscountAmount,
                FinalPrice = e.FinalPrice,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt,
                Version = e.Version
            };
        }

        public async Task AddAsync(BookingDTOs dto)
        {
            var entity = new Booking
            {
                Id = dto.Id,
                UserId = dto.UserId,
                ShowtimeId = dto.ShowtimeId,
                Status = dto.Status,
                PaymentMethod = dto.PaymentMethod,
                TransactionId = dto.TransactionId,
                TotalPrice = dto.TotalPrice,
                DiscountAmount = dto.DiscountAmount,
                FinalPrice = dto.FinalPrice,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                Version = dto.Version
            };
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(BookingDTOs dto)
        {
            var entity = new Booking
            {
                Id = dto.Id,
                UserId = dto.UserId,
                ShowtimeId = dto.ShowtimeId,
                Status = dto.Status,
                PaymentMethod = dto.PaymentMethod,
                TransactionId = dto.TransactionId,
                TotalPrice = dto.TotalPrice,
                DiscountAmount = dto.DiscountAmount,
                FinalPrice = dto.FinalPrice,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                Version = dto.Version
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
