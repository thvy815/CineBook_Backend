using Microsoft.AspNetCore.Mvc;
using MovieService.Domain.Entities;
using MovieService.Application.Services;

namespace MovieService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieDetailController : ControllerBase
    {
        private readonly MovieDetailService _service;

        public MovieDetailController(MovieDetailService service)
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
        public async Task<IActionResult> Create(MovieDetail movie)
        {
            await _service.AddAsync(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MovieDetail movie)
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
