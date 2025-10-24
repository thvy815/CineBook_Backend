using System;

namespace UserProfileService.Domain.DTOs
{
	public class FavoriteMovieCreateDTO
	{
		public int TmdbId { get; set; }
	}

	public class FavoriteMovieReadDTO
	{
		public Guid Id { get; set; }
		public Guid UserProfileId { get; set; }
		public int TmdbId { get; set; }
		public DateTime AddedAt { get; set; }
	}
}
