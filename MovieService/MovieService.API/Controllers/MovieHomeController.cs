using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieService.Infrastructure.Data;

namespace MovieService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieHomeController : ControllerBase
    {
        private readonly MovieDbContext _db;

        public MovieHomeController(MovieDbContext db)
        {
            _db = db;
        }

        // 🎥 GET: /api/moviehome/now-playing
        [HttpGet("now-playing")]
        public async Task<IActionResult> GetNowPlaying()
        {
            var movies = await _db.MovieDetails
                .Where(m => m.Status == "NowPlaying")
                .OrderByDescending(m => m.ReleaseDate)
                .Take(40)
                .ToListAsync();

            return Ok(movies);
        }

        // 🎬 GET: /api/moviehome/upcoming
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var movies = await _db.MovieDetails
                .Where(m => m.Status == "Upcoming")
                .OrderBy(m => m.ReleaseDate)
                .Take(40)
                .ToListAsync();

            return Ok(movies);
        }
    }
}
