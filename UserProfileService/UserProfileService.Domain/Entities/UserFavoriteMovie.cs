using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileService.Domain.Entities
{
	[Table("user_favorite_movies")]
	public class UserFavoriteMovie
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Column("user_profile_id")]
		public Guid UserProfileId { get; set; }

		[Column("tmdb_id")]
		public int TmdbId { get; set; }

		[Column("added_at")]
		public DateTime AddedAt { get; set; }

		// Navigation
		public UserProfile UserProfile { get; set; }
	}
}
