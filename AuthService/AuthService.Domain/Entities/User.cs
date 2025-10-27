using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
	[Table("users")]
	public class User
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required]
		[Column("role_id")]
		public int RoleId { get; set; }
		public Role Role { get; set; }

		[Required, MaxLength(100)]
		[Column("email")]
		public string Email { get; set; }

		[MaxLength(30)]
		[Column("username")]
		public string Username { get; set; }

		[Column("fullname")]
		[MaxLength(100)]
		public string Fullname { get; set; }

		[Column("gender")]
		[MaxLength(10)]
		public string Gender { get; set; }

		[Column("date_of_birth", TypeName = "date")]
		public DateTime DateOfBirth { get; set; }

		[MaxLength(15)]
		[Column("phone_number")]
		public string PhoneNumber { get; set; }

		[MaxLength(20)]
		[Column("national_id")]
		public string NationalId { get; set; }

		[Required, MaxLength(255)]
		[Column("password_hash")]
		public string PasswordHash { get; set; }

		[MaxLength(20)]
		[Column("status")]
		public string Status { get; set; } = "ACTIVE"; // ACTIVE, BANNED

		[Column("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Column("updated_at")]
		public DateTime? UpdatedAt { get; set; }

		public ICollection<RefreshToken> RefreshTokens { get; set; }
	}
}
