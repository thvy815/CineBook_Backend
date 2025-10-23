using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
	[Table("password_reset_otps")]
	public class PasswordResetOtp
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required, MaxLength(100)]
		[Column("email")]
		public string Email { get; set; }

		[Required, MaxLength(10)]
		[Column("otp")]
		public string Otp { get; set; }

		[Column("expires_at")]
		public DateTime ExpiresAt { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
