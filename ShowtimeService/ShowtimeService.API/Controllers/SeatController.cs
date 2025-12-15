using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly SeatService _service;
        public SeatController(SeatService service) => _service = service;

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetByRoom(Guid roomId)
            => Ok(await _service.GetByRoomAsync(roomId));

        [HttpPost]
        public async Task<IActionResult> Create(CreateSeatDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSeats([FromBody] CreateSeatsRequest request)
        {
            var result = await _service.GenerateSeatsAsync(request);
            return Ok(result);
        }
    }


   
}
