using Microsoft.AspNetCore.JsonPatch;
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
			var res = await _business.GetByUserIdAsync(userId);
			if (res == null) return NotFound();
			return Ok(res);
		}

		[HttpPut("by-user/{userId}")]
		public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UserProfileUpdateDTO dto)
		{
			var res = await _business.UpdateProfileAsync(userId, dto);
			if (res == null) return NotFound();
			return Ok(res);
		}

		// Favorites
		[HttpGet("{userId}/favorites")]
		public async Task<IActionResult> GetFavorites(Guid userId)
		{
			var list = await _business.GetFavoritesByAuthUserIdAsync(userId);
			return Ok(list);
		}

		[HttpPost("{userId}/favorites")]
		public async Task<IActionResult> AddFavorite(Guid userId, [FromBody] FavoriteMovieCreateDTO dto)
		{
			var ok = await _business.AddFavoriteByAuthUserIdAsync(userId, dto.TmdbId);
			if (!ok) return NotFound(); // profile not found
			return NoContent();
		}

		[HttpDelete("{userId}/favorites/{tmdbId}")]
		public async Task<IActionResult> RemoveFavorite(Guid userId, int tmdbId)
		{
			var ok = await _business.RemoveFavoriteByAuthUserIdAsync(userId, tmdbId);
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
			var rank = await _business.GetRankForAuthUserAsync(userId);
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
