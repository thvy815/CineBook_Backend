using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProfileService.Domain.DTOs;
using UserProfileService.Domain.Entities;
using UserProfileService.Domain.Interfaces;

namespace UserProfileService.Application.Services
{
	public class UserProfileBusiness
	{
		private readonly IUserProfileRepository _repo;

		public UserProfileBusiness(IUserProfileRepository repo)
		{
			_repo = repo;
		}

		// Get profile by auth user_id
		public async Task<UserProfileReadDTO> GetByUserIdAsync(Guid userId)
		{
			var p = await _repo.GetByUserIdAsync(userId);
			return MapToReadDto(p);
		}

		public async Task<UserProfileReadDTO> UpdateProfileAsync(Guid userId, UserProfileUpdateDTO update)
		{
			var p = await _repo.GetByUserIdAsync(userId);
			if (p == null) return null;

			p.Email = update.Email ?? p.Email;
			p.Username = update.Username ?? p.Username;
			p.Fullname = update.Fullname ?? p.Fullname;
			p.AvatarUrl = update.AvatarUrl ?? p.AvatarUrl;
			p.Gender = update.Gender ?? p.Gender;
			p.DateOfBirth = update.DateOfBirth ?? p.DateOfBirth;
			p.PhoneNumber = update.PhoneNumber ?? p.PhoneNumber;
			p.NationalId = update.NationalId ?? p.NationalId;
			p.Address = update.Address ?? p.Address;
			p.Status = update.Status ?? p.Status;
			p.UpdatedAt = DateTime.UtcNow;

			var updated = await _repo.UpdateAsync(p);
			return MapToReadDto(updated);
		}

		// Create Profile if not exists
		// chỉ dùng nội bộ, trả entity cho các repo hoặc logic khác
		private async Task<UserProfile> GetOrCreateEntityAsync(Guid userId, UserProfileCreateDTO? dto = null)
		{
			var profile = await _repo.GetByUserIdAsync(userId);
			if (profile == null)
			{
				profile = new UserProfile
				{
					Id = Guid.NewGuid(),
					UserId = userId,
					Email = dto?.Email,
					Username = dto?.Username,
					Fullname = dto?.Fullname,
					AvatarUrl = dto?.AvatarUrl,
					Gender = dto?.Gender,
					DateOfBirth = dto?.DateOfBirth ?? default(DateTime),
					PhoneNumber = dto?.PhoneNumber,
					NationalId = dto?.NationalId,
					Address = dto?.Address,
					Status = dto?.Status ?? "ACTIVE",
					CreatedAt = DateTime.UtcNow,
					UpdatedAt = DateTime.UtcNow
				};
				await _repo.AddAsync(profile);
			}
			return profile;
		}

		// public cho Controller, trả DTO
		public async Task<UserProfileReadDTO> CreateIfNotExistsAsync(Guid userId, UserProfileCreateDTO? dto = null)
		{
			var entity = await GetOrCreateEntityAsync(userId, dto);
			return MapToReadDto(entity);
		}

		// Favorites: operate by auth user_id (convert to user_profile_id internally)
		public async Task<IEnumerable<FavoriteMovieReadDTO>> GetFavoritesAsync(Guid userProfileId)
		{
			var favs = await _repo.GetFavoritesByUserProfileIdAsync(userProfileId);
			return favs.Select(f => new FavoriteMovieReadDTO
			{
				Id = f.Id,
				UserProfileId = f.UserProfileId,
				TmdbId = f.TmdbId,
				AddedAt = f.AddedAt
			});
		}

		public async Task<bool> AddFavoriteAsync(Guid userProfileId, int tmdbId)
		{
			// avoid duplicates
			var exists = await _repo.GetFavoriteByProfileAndTmdbAsync(userProfileId, tmdbId);
			if (exists != null) return true; // idempotent

			var fav = new UserFavoriteMovie
			{
				Id = Guid.NewGuid(),
				UserProfileId = userProfileId,
				TmdbId = tmdbId,
				AddedAt = DateTime.UtcNow
			};

			await _repo.AddFavoriteAsync(fav);
			return true;
		}

		public async Task<bool> RemoveFavoriteAsync(Guid userProfileId, int tmdbId)
		{
			var exists = await _repo.GetFavoriteByProfileAndTmdbAsync(userProfileId, tmdbId);
			if (exists == null) return true; // already removed (idempotent)

			await _repo.RemoveFavoriteAsync(exists);
			return true;
		}

		// Ranks
		public async Task<IEnumerable<UserRankDTO>> GetAllRanksAsync()
		{
			var ranks = await _repo.GetAllRanksAsync();
			return ranks.Select(r => new UserRankDTO
			{
				Id = r.Id,
				Name = r.Name,
				MinPoints = r.MinPoints,
				MaxPoints = r.MaxPoints,
				DiscountRate = r.DiscountRate
			});
		}

		public async Task<UserRankDTO> GetRankForUserProfileAsync(Guid userProfileId)
		{
			var profile = await _repo.GetByUserProfileIdAsync(userProfileId);
			if (profile == null) return null;

			var rank = await _repo.GetRankByPointsAsync(profile.LoyaltyPoint);
			if (rank == null) return null;

			return new UserRankDTO
			{
				Id = rank.Id,
				Name = rank.Name,
				MinPoints = rank.MinPoints,
				MaxPoints = rank.MaxPoints,
				DiscountRate = rank.DiscountRate
			};
		}

		// Manager / Staff
		public async Task<IEnumerable<ManagerProfileDTO>> GetManagersByCinemaAsync(Guid cinemaId)
		{
			var list = await _repo.GetManagersByCinemaAsync(cinemaId);
			return list.Select(m => new ManagerProfileDTO
			{
				Id = m.Id,
				UserProfileId = m.UserProfileId,
				ManagedCinemaId = m.ManagedCinemaId,
				HireDate = m.HireDate,
				UserProfile = MapToReadDto(m.UserProfile)
			});
		}

		public async Task<IEnumerable<StaffProfileDTO>> GetStaffByCinemaAsync(Guid cinemaId)
		{
			var list = await _repo.GetStaffByCinemaAsync(cinemaId);
			return list.Select(s => new StaffProfileDTO
			{
				Id = s.Id,
				UserProfileId = s.UserProfileId,
				ManagedCinemaId = s.ManagedCinemaId,
				HireDate = s.HireDate,
				UserProfile = MapToReadDto(s.UserProfile)
			});
		}

		public async Task<IEnumerable<ManagerProfileDTO>> GetAllManagersAsync()
		{
			var list = await _repo.GetAllManagersAsync();
			return list.Select(m => new ManagerProfileDTO
			{
				Id = m.Id,
				UserProfileId = m.UserProfileId,
				ManagedCinemaId = m.ManagedCinemaId,
				HireDate = m.HireDate,
				UserProfile = MapToReadDto(m.UserProfile)
			});
		}

		public async Task<IEnumerable<StaffProfileDTO>> GetAllStaffAsync()
		{
			var list = await _repo.GetAllStaffAsync();
			return list.Select(s => new StaffProfileDTO
			{
				Id = s.Id,
				UserProfileId = s.UserProfileId,
				ManagedCinemaId = s.ManagedCinemaId,
				HireDate = s.HireDate,
				UserProfile = MapToReadDto(s.UserProfile)
			});
		}

		// helper mapping
		private UserProfileReadDTO MapToReadDto(UserProfile p)
		{
			if (p == null) return null;
			var dto = new UserProfileReadDTO
			{
				Id = p.Id,
				UserId = p.UserId,
				RankId = p.RankId,
				Email = p.Email,
				Username = p.Username,
				Fullname = p.Fullname,
				AvatarUrl = p.AvatarUrl,
				Gender = p.Gender,
				DateOfBirth = p.DateOfBirth,
				PhoneNumber = p.PhoneNumber,
				NationalId = p.NationalId,
				Address = p.Address,
				LoyaltyPoint = p.LoyaltyPoint,
				Status = p.Status,
				CreatedAt = p.CreatedAt,
				UpdatedAt = p.UpdatedAt
			};

			if (p.Rank != null)
			{
				dto.Rank = new UserRankDTO
				{
					Id = p.Rank.Id,
					Name = p.Rank.Name,
					MinPoints = p.Rank.MinPoints,
					MaxPoints = p.Rank.MaxPoints,
					DiscountRate = p.Rank.DiscountRate
				};
			}

			return dto;
		}
	}
}
