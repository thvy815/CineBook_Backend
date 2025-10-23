using Microsoft.AspNetCore.Mvc;
using MovieService.Domain.Entities;
using MovieService.Application.Services;

namespace MovieService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieSummaryController : ControllerBase
    {
        private readonly MovieSummaryService _service;

        public MovieSummaryController(MovieSummaryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieSummary movie)
        {
            await _service.AddAsync(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MovieSummary movie)
        {
            if (id != movie.Id) return BadRequest();
            await _service.UpdateAsync(movie);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
