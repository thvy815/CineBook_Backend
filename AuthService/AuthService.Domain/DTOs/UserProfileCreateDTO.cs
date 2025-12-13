using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.DTOs
{
	public class UserProfileCreateDTO
	{
		public string Email { get; set; }
		public string Username { get; set; }
		public string Fullname { get; set; }
		public string? AvatarUrl { get; set; }
		public string? Gender { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string PhoneNumber { get; set; }
		public string? NationalId { get; set; }
		public string? Address { get; set; }
		public int LoyaltyPoint { get; set; } = 0;
		public string Status { get; set; } = "ACTIVE";
	}
}
