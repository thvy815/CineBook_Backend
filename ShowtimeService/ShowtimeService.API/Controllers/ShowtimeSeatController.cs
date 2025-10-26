using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeSeatController : ControllerBase
    {
        private readonly ShowtimeSeatService _service;
        public ShowtimeSeatController(ShowtimeSeatService service) => _service = service;

        [HttpGet("{showtimeId}")]
        public async Task<IActionResult> GetByShowtime(Guid showtimeId)
            => Ok(await _service.GetByShowtimeAsync(showtimeId));

        [HttpPost]
        public async Task<IActionResult> Create(CreateShowtimeSeatDto dto)
            => Ok(await _service.CreateAsync(dto));
    }
}
