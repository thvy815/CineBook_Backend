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

		[HttpPost]
		public async Task<IActionResult> Add(Movie movie)
		{
			await _movieBusiness.AddAsync(movie);
			return Ok();
		}
	}
}
