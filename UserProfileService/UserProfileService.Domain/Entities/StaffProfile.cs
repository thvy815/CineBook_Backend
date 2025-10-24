using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileService.Domain.Entities
{
	[Table("staff_profiles")]
	public class StaffProfile
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Column("user_profile_id")]
		public Guid UserProfileId { get; set; }

		[Column("managed_cinema_id")]
		public Guid ManagedCinemaId { get; set; }

		[Column("hire_date", TypeName = "date")]
		public DateTime? HireDate { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime? UpdatedAt { get; set; }

		public UserProfile UserProfile { get; set; }
	}
}
