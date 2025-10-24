using System;

namespace UserProfileService.Domain.DTOs
{
	public class UserProfileReadDTO
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }     // AuthService ID
		public Guid? RankId { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public string Fullname { get; set; }
		public string AvatarUrl { get; set; }
		public string Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string NationalId { get; set; }
		public string Address { get; set; }
		public int LoyaltyPoint { get; set; }
		public string Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }

		public UserRankDTO Rank { get; set; }
	}

	public class UserProfileUpdateDTO
	{
		public string Email { get; set; }
		public string Username { get; set; }
		public string Fullname { get; set; }
		public string AvatarUrl { get; set; }
		public string Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string NationalId { get; set; }
		public string Address { get; set; }
		public string Status { get; set; }
	}
}
