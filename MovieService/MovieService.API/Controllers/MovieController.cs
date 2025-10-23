using Microsoft.AspNetCore.Mvc;
using MovieService.Application.Services;
using MovieService.Domain.Entities;

namespace MovieService.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class MovieController : ControllerBase
	{
		private readonly MovieBusiness _movieBusiness;

		public MovieController(MovieBusiness movieBusiness)
		{
			_movieBusiness = movieBusiness;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll() => Ok(await _movieBusiness.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _movieBusiness.GetByIdAsync(id);
            if (movie == null)
                return NotFound();
            return Ok(movie);
        }

        [HttpPost]
		public async Task<IActionResult> Add(Movie movie)
		{
			await _movieBusiness.AddAsync(movie);
			return Ok();
		}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Movie movie)
        {
            if (id != movie.Id) return BadRequest();
            await _movieBusiness.UpdateMovieAsync(movie);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _movieBusiness.DeleteMovieAsync(id);
            return NoContent();
        }
    }
}
