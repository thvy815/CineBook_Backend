using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class TheaterService
    {
        private readonly ITheaterRepository _theaterRepository;

        public TheaterService(ITheaterRepository theaterRepository)
        {
            _theaterRepository = theaterRepository;
        }

        public async Task<IEnumerable<Theater>> GetAllAsync()
            => await _theaterRepository.GetAllAsync();

        public async Task<Theater> GetByIdAsync(Guid id)
            => await _theaterRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Theater>> GetByProvinceIdAsync(Guid provinceId)
            => await _theaterRepository.GetByProvinceIdAsync(provinceId);

        public async Task AddAsync(Theater theater)
            => await _theaterRepository.AddAsync(theater);

        public async Task UpdateAsync(Theater theater)
            => await _theaterRepository.UpdateAsync(theater);

        public async Task DeleteAsync(Guid id)
            => await _theaterRepository.DeleteAsync(id);
    }
}
