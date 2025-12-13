using Microsoft.AspNetCore.Mvc;
using ShowtimeService.Application.DTOs;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterController : ControllerBase
    {
        private readonly TheaterService _service;
        public TheaterController(TheaterService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create(CreateTheaterDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpGet("filter-by-province")]
        public async Task<IActionResult> GetTheatersByProvinceAndDate(
        [FromQuery] Guid? provinceId,
        [FromQuery] string date)
        {
            try
            {
                var theaters = await _service.GetByProvinceWithShowtimeAsync(provinceId, date);
                return Ok(theaters);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }



}
