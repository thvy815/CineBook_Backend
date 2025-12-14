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

        // 🟢 Lấy danh sách tất cả phim
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _service.GetAllAsync();
            return Ok(movies);
        }

        // 🟢 Lấy chi tiết phim theo ID
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Movie with ID {id} not found." });
            return Ok(result);
        }

        // 🟢 Tạo phim mới (nếu nhập tay)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovieDetail movie)
        {
            if (movie == null)
                return BadRequest(new { message = "Invalid movie data." });

            await _service.AddAsync(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        // 🟡 Cập nhật thông tin phim
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MovieDetail movie)
        {
            if (id != movie.Id)
                return BadRequest(new { message = "Movie ID mismatch." });

            var updated = await _service.UpdateAsync(movie);
            if (!updated)
                return NotFound(new { message = $"Movie with ID {id} not found." });

            return Ok(new { message = "Movie updated successfully." });
        }

        // 🔴 Xóa phim
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Movie with ID {id} not found." });

            return Ok(new { message = "Movie deleted successfully." });
        }

        // 🔍 Tìm kiếm phim theo từ khóa
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required." });

            var results = await _service.SearchAsync(keyword);
            if (results == null || !results.Any())
                return NotFound(new { message = "No movies found matching the keyword." });

            return Ok(results);
        }

        // 🎬 Lọc theo trạng thái (đang chiếu hoặc sắp chiếu)
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var results = await _service.GetByStatusAsync(status);
            if (results == null || !results.Any())
                return NotFound(new { message = $"No movies found with status '{status}'." });

            return Ok(results);
        }

        // 🎯 Admin search / advanced search
        [HttpGet("advanced-search")]
        public async Task<IActionResult> AdvancedSearch(
            [FromQuery] string? keyword,
            [FromQuery] string? status,
            [FromQuery] string? genres,
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortType = null)
        {
            // TODO: check role admin/manager
            //if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            //    return Forbid();

            var result = await _service.AdminSearchAsync(keyword, status, genres, page, size, sortBy, sortType);
            return Ok(result);
        }
    }
}
