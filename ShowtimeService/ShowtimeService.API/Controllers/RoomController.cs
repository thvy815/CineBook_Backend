using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly RoomService _service;
        public RoomController(RoomService service) => _service = service;

        [HttpGet("{theaterId}")]
        public async Task<IActionResult> GetByTheater(Guid theaterId)
            => Ok(await _service.GetByTheaterIdAsync(theaterId));

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoomDto dto)
            => Ok(await _service.CreateAsync(dto));
    }
}
