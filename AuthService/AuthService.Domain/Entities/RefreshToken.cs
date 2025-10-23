using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities
{
	[Table("refresh_tokens")]
	public class RefreshToken
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Required]
		[Column("user_id")]
		public Guid UserId { get; set; }
		public User User { get; set; }

		[Required]
		[Column("token")]
		public string Token { get; set; }

		[Column("expires_at")]
		public DateTime ExpiresAt { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
