using ShowtimeService.Domain.Entities;
using ShowtimeService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShowtimeService.Application.Services
{
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<IEnumerable<Room>> GetAllAsync()
            => await _roomRepository.GetAllAsync();

        public async Task<Room> GetByIdAsync(Guid id)
            => await _roomRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Room>> GetByTheaterIdAsync(Guid theaterId)
            => await _roomRepository.GetByTheaterIdAsync(theaterId);

        public async Task AddAsync(Room room)
            => await _roomRepository.AddAsync(room);

        public async Task UpdateAsync(Room room)
            => await _roomRepository.UpdateAsync(room);

        public async Task DeleteAsync(Guid id)
            => await _roomRepository.DeleteAsync(id);
    }
}
