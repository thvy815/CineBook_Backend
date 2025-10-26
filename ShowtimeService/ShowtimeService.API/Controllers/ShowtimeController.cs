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
    }
}
