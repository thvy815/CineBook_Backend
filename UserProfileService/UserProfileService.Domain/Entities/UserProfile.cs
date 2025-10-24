using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileService.Domain.Entities
{
	[Table("user_profiles")]
	public class UserProfile
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		// ID from AuthService
		[Column("user_id")]
		public Guid UserId { get; set; }

		[Column("rank_id")]
		public Guid? RankId { get; set; }

		[Column("email")]
		[MaxLength(100)]
		public string Email { get; set; }

		[Column("username")]
		[MaxLength(30)]
		public string Username { get; set; }

		[Column("fullname")]
		[MaxLength(100)]
		public string Fullname { get; set; }

		[Column("avatar_url")]
		public string AvatarUrl { get; set; }

		[Column("gender")]
		[MaxLength(10)]
		public string? Gender { get; set; }

		[Column("date_of_birth", TypeName = "date")]
		public DateTime DateOfBirth { get; set; }

		[Column("phone_number")]
		[MaxLength(15)]
		public string PhoneNumber { get; set; }

		[Column("national_id")]
		[MaxLength(20)]
		public string NationalId { get; set; }

		[Column("address")]
		public string? Address { get; set; }

		[Column("loyalty_point")]
		public int LoyaltyPoint { get; set; }

		[Column("status")]
		[MaxLength(20)]
		public string Status { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime? UpdatedAt { get; set; }

		// Navigation
		public UserRank Rank { get; set; }

		// Favorites navigation
		public ICollection<UserFavoriteMovie> FavoriteMovies { get; set; } = new List<UserFavoriteMovie>();
	}
}
