using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShowtimeController : ControllerBase
    {
        private readonly ShowtimeBusiness _business;

        public ShowtimeController(ShowtimeBusiness business)
        {
            _business = business;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _business.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _business.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeDto dto)
        {
            var result = await _business.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateShowtimeDto dto)
        {
            var result = await _business.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return await _business.DeleteAsync(id) ? Ok() : NotFound();
        }

        [HttpPost("generate-auto")]
        public async Task<IActionResult> Generate(Guid theaterId, Guid roomId)
        {
            await _business.GenerateAutoShowtimesAsync(theaterId, roomId);

            return Ok(new
            {
                message = "Đã tự động tạo suất chiếu cho 5 ngày tiếp theo."
            });
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
           [FromQuery] Guid? theaterId,
           [FromQuery] Guid? movieId,
           [FromQuery] string? date)
        {
            var result = await _business.FilterAsync(theaterId, movieId, date);
            return Ok(result);
        }

        [HttpGet("filterByTheaterAndDate")]
        public async Task<IActionResult> FilterShowtimes(
        [FromQuery] Guid? theaterId,
        [FromQuery] Guid? movieId,
        [FromQuery] string date)
            {
                try
                {
                    var showtimes = await _business.FilterShowtimesAsync(theaterId, movieId, date);
                    return Ok(showtimes);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }

        [HttpGet("filterByAll")]
        public async Task<IActionResult> Filter(
                Guid provinceId,
                Guid movieId,
                string date)
        {
            var data = await _business
                .FilterShowtimeAsync(provinceId, movieId, date);

            return Ok(data);
        }
    }
}
