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
    public class BookingFnbBusiness
    {
        private readonly IBookingFnbRepository _repository;

        public BookingFnbBusiness(IBookingFnbRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BookingFnbDTOs>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => new BookingFnbDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                FnbItemId = e.FnbItemId,
                Quantity = e.Quantity,
                UnitPrice = e.UnitPrice,
                TotalFnbPrice = e.TotalFnbPrice
            });
        }

        public async Task<BookingFnbDTOs?> GetByIdAsync(Guid id)
        {
            var e = await _repository.GetByIdAsync(id);
            if (e == null) return null;

            return new BookingFnbDTOs
            {
                Id = e.Id,
                BookingId = e.BookingId,
                FnbItemId = e.FnbItemId,
                Quantity = e.Quantity,
                UnitPrice = e.UnitPrice,
                TotalFnbPrice = e.TotalFnbPrice
            };
        }

        public async Task AddAsync(BookingFnbDTOs dto)
        {
            var entity = new BookingFnb
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                FnbItemId = dto.FnbItemId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalFnbPrice = dto.TotalFnbPrice
            };
            await _repository.AddAsync(entity);
        }
        public async Task UpdateAsync(BookingFnbDTOs dto)
        {
            var entity = new BookingFnb
            {
                Id = dto.Id,
                BookingId = dto.BookingId,
                FnbItemId = dto.FnbItemId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalFnbPrice = dto.TotalFnbPrice
            };

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
