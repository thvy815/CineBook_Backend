using Microsoft.AspNetCore.Mvc;
using MovieService.Application.Services;

namespace MovieService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly TmdbService _tmdbService;

        public ImportController(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

 
        [HttpGet("tmdb/{id}")]
        public async Task<IActionResult> GetFromTmdb(int id)
        {
            try
            {
                var movie = await _tmdbService.GetFromTmdbAsync(id);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // ✅ POST: /api/import/tmdb/550
        [HttpPost("tmdb/{id}")]
        public async Task<IActionResult> ImportFromTmdb(int id)
        {
            try
            {
                var movie = await _tmdbService.GetFromTmdbAsync(id);
                return Ok(new
                {
                    message = $"Movie with TMDB ID {id} imported successfully!",
                    data = movie
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        //Đồng bộ phim đang chiếu và sắp chiếu (40)
        [HttpPost("sync-all")]
        public async Task<IActionResult> SyncAllMovies()
        {
            try
            {
                await _tmdbService.SyncNowPlayingAndUpcomingAsync();
                return Ok(new { message = "✅ Synced Now Playing and Upcoming movies successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




    }
}
