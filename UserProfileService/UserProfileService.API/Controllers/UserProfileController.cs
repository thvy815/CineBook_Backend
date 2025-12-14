using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserProfileService.Application.Services;
using UserProfileService.Domain.DTOs;

namespace UserProfileService.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserProfileController : ControllerBase
	{
		private readonly UserProfileBusiness _business;

		public UserProfileController(UserProfileBusiness business)
		{
			_business = business;
		}

		[HttpGet("by-user/{userId}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var res = await _business.CreateIfNotExistsAsync(userId); // Lấy hoặc tạo 1 lần
			if (res == null) return NotFound();
			return Ok(res);
		}

		[HttpPost("by-user/{userId}")]
		public async Task<IActionResult> CreateProfile(Guid userId, [FromBody] UserProfileCreateDTO dto)
		{ 
			var res = await _business.CreateIfNotExistsAsync(userId, dto);
			return Ok(res);
		}

		[HttpPut("by-user/{userId}")]
		public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UserProfileUpdateDTO dto)
		{
			var res = await _business.CreateIfNotExistsAsync(userId);
			if (res == null) return NotFound();

			var updated = await _business.UpdateProfileAsync(userId, dto);
			return Ok(updated);
		}

        [HttpPut("{userId}/avatar")]
        public async Task<IActionResult> UpdateAvatar(Guid userId, [FromBody] UpdateAvatarDto dto)
        {
            try
            {
                var res = await _business.UpdateAvatarAsync(userId, dto.AvatarUrl);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Favorites
        [HttpGet("{userId}/favorites")]
		public async Task<IActionResult> GetFavorites(Guid userId)
		{
			var profile = await _business.CreateIfNotExistsAsync(userId);
			var list = await _business.GetFavoritesAsync(profile.Id);
			return Ok(list);
		}

		[HttpPost("{userId}/favorites")]
		public async Task<IActionResult> AddFavorite(Guid userId, [FromBody] FavoriteMovieCreateDTO dto)
		{
			var profile = await _business.CreateIfNotExistsAsync(userId);
			var ok = await _business.AddFavoriteAsync(profile.Id, dto.TmdbId);
			if (!ok) return NotFound(); // profile not found
			return NoContent();
		}

		[HttpDelete("{userId}/favorites/{tmdbId}")]
		public async Task<IActionResult> RemoveFavorite(Guid userId, int tmdbId)
		{
			var profile = await _business.CreateIfNotExistsAsync(userId);
			var ok = await _business.RemoveFavoriteAsync(profile.Id, tmdbId);
			if (!ok) return NotFound();
			return NoContent();
		}

		// Ranks
		[HttpGet("ranks")]
		public async Task<IActionResult> GetAllRanks()
		{
			var ranks = await _business.GetAllRanksAsync();
			return Ok(ranks);
		}

		[HttpGet("{userId}/rank")]
		public async Task<IActionResult> GetRankForUser(Guid userId)
		{
			var profile = await _business.CreateIfNotExistsAsync(userId);
			var rank = await _business.GetRankForUserProfileAsync(profile.Id);
			if (rank == null) return NotFound();
			return Ok(rank);
		}

		// Managers / Staff
		[HttpGet("cinema/{cinemaId}/managers")]
		public async Task<IActionResult> GetManagersByCinema(Guid cinemaId)
		{
			var list = await _business.GetManagersByCinemaAsync(cinemaId);
			return Ok(list);
		}

		[HttpGet("cinema/{cinemaId}/staff")]
		public async Task<IActionResult> GetStaffByCinema(Guid cinemaId)
		{
			var list = await _business.GetStaffByCinemaAsync(cinemaId);
			return Ok(list);
		}

		[HttpGet("managers")]
		public async Task<IActionResult> GetAllManagers()
		{
			var list = await _business.GetAllManagersAsync();
			return Ok(list);
		}

		[HttpGet("staff")]
		public async Task<IActionResult> GetAllStaff()
		{
			var list = await _business.GetAllStaffAsync();
			return Ok(list);
		}
	}
}
