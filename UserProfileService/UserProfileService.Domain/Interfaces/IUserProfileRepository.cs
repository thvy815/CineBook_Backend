using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserProfileService.Domain.Entities;

namespace UserProfileService.Domain.Interfaces
{
	public interface IUserProfileRepository
	{
		// Profiles
		Task<UserProfile> GetByUserIdAsync(Guid userId); // from AuthService
		Task<UserProfile> GetByUserProfileIdAsync(Guid userProfileId);
		Task<UserProfile> GetByIdAsync(Guid id);
		Task<UserProfile> AddAsync(UserProfile profile);
		Task<UserProfile> UpdateAsync(UserProfile profile);

		// Favorites (use user_profile_id internally)
		Task<IEnumerable<UserFavoriteMovie>> GetFavoritesByUserProfileIdAsync(Guid userProfileId);
		Task<UserFavoriteMovie> GetFavoriteByProfileAndTmdbAsync(Guid userProfileId, int tmdbId);
		Task AddFavoriteAsync(UserFavoriteMovie favorite);
		Task RemoveFavoriteAsync(UserFavoriteMovie favorite);

		// Ranks
		Task<IEnumerable<UserRank>> GetAllRanksAsync();
		Task<UserRank> GetRankByIdAsync(Guid id);
		Task<UserRank> GetRankByPointsAsync(int points);

		// Manager / Staff
		Task<IEnumerable<ManagerProfile>> GetManagersByCinemaAsync(Guid cinemaId);
		Task<IEnumerable<StaffProfile>> GetStaffByCinemaAsync(Guid cinemaId);
		Task<IEnumerable<ManagerProfile>> GetAllManagersAsync();
		Task<IEnumerable<StaffProfile>> GetAllStaffAsync();
	}
}
