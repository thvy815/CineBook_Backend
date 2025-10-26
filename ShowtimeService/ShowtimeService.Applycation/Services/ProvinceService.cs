using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class ProvinceService
    {
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IProvinceRepository provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        public async Task<IEnumerable<Province>> GetAllAsync()
            => await _provinceRepository.GetAllAsync();

        public async Task<Province> GetByIdAsync(Guid id)
            => await _provinceRepository.GetByIdAsync(id);

        public async Task<Province> GetByNameAsync(string name)
            => await _provinceRepository.GetByNameAsync(name);

        public async Task AddAsync(Province province)
            => await _provinceRepository.AddAsync(province);

        public async Task UpdateAsync(Province province)
            => await _provinceRepository.UpdateAsync(province);

        public async Task DeleteAsync(Guid id)
            => await _provinceRepository.DeleteAsync(id);
    }
}
