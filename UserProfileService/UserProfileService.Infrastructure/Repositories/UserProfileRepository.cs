using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserProfileService.Domain.Entities;
using UserProfileService.Domain.Interfaces;
using UserProfileService.Infrastructure.Data;

namespace UserProfileService.Infrastructure.Repositories
{
	public class UserProfileRepository : IUserProfileRepository
	{
		private readonly UserProfileDbContext _db;
		public UserProfileRepository(UserProfileDbContext db) => _db = db;

		// Profiles
		public async Task<UserProfile> GetByUserIdAsync(Guid userId)
		{
			return await _db.UserProfiles
				.Include(p => p.Rank)
				.Include(p => p.FavoriteMovies)
				.FirstOrDefaultAsync(p => p.UserId == userId);
		}

		public async Task<UserProfile> GetByUserProfileIdAsync(Guid userProfileId)
		{
			return await _db.UserProfiles
				.Include(p => p.Rank)
				.Include(p => p.FavoriteMovies)
				.FirstOrDefaultAsync(p => p.Id == userProfileId);
		}

		public async Task<UserProfile> GetByIdAsync(Guid id)
		{
			return await _db.UserProfiles
				.Include(p => p.Rank)
				.Include(p => p.FavoriteMovies)
				.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<UserProfile> AddAsync(UserProfile profile)
		{
			_db.UserProfiles.Add(profile);
			await _db.SaveChangesAsync();
			return profile;
		}

		public async Task<UserProfile> UpdateAsync(UserProfile profile)
		{
			_db.UserProfiles.Update(profile);
			await _db.SaveChangesAsync();
			return profile;
		}

		// Favorites (use user_profile_id)
		public async Task<IEnumerable<UserFavoriteMovie>> GetFavoritesByUserProfileIdAsync(Guid userProfileId)
		{
			return await _db.UserFavoriteMovies
				.Where(f => f.UserProfileId == userProfileId)
				.ToListAsync();
		}

		public async Task<UserFavoriteMovie> GetFavoriteByProfileAndTmdbAsync(Guid userProfileId, int tmdbId)
		{
			return await _db.UserFavoriteMovies
				.FirstOrDefaultAsync(f => f.UserProfileId == userProfileId && f.TmdbId == tmdbId);
		}

		public async Task AddFavoriteAsync(UserFavoriteMovie favorite)
		{
			_db.UserFavoriteMovies.Add(favorite);
			await _db.SaveChangesAsync();
		}

		public async Task RemoveFavoriteAsync(UserFavoriteMovie favorite)
		{
			_db.UserFavoriteMovies.Remove(favorite);
			await _db.SaveChangesAsync();
		}

		// Ranks
		public async Task<IEnumerable<UserRank>> GetAllRanksAsync()
		{
			return await _db.UserRanks.OrderBy(r => r.MinPoints).ToListAsync();
		}

		public async Task<UserRank> GetRankByIdAsync(Guid id)
		{
			return await _db.UserRanks.FindAsync(id);
		}

		public async Task<UserRank> GetRankByPointsAsync(int points)
		{
			return await _db.UserRanks.FirstOrDefaultAsync(r => r.MinPoints <= points && r.MaxPoints >= points);
		}

		// Manager / Staff
		public async Task<IEnumerable<ManagerProfile>> GetManagersByCinemaAsync(Guid cinemaId)
		{
			return await _db.ManagerProfiles.Include(m => m.UserProfile).Where(m => m.ManagedCinemaId == cinemaId).ToListAsync();
		}

		public async Task<IEnumerable<StaffProfile>> GetStaffByCinemaAsync(Guid cinemaId)
		{
			return await _db.StaffProfiles.Include(s => s.UserProfile).Where(s => s.ManagedCinemaId == cinemaId).ToListAsync();
		}

		public async Task<IEnumerable<ManagerProfile>> GetAllManagersAsync()
		{
			return await _db.ManagerProfiles.Include(m => m.UserProfile).ToListAsync();
		}

		public async Task<IEnumerable<StaffProfile>> GetAllStaffAsync()
		{
			return await _db.StaffProfiles.Include(s => s.UserProfile).ToListAsync();
		}
	}
}
