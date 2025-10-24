using System;

namespace UserProfileService.Domain.DTOs
{
	public class ManagerProfileDTO
	{
		public Guid Id { get; set; }
		public Guid UserProfileId { get; set; }
		public Guid ManagedCinemaId { get; set; }
		public DateTime? HireDate { get; set; }
		public UserProfileReadDTO UserProfile { get; set; }
	}
}
