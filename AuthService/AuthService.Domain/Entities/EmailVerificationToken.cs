using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Domain.Entities
{
    [Table("email_verification_tokens")]
    public class EmailVerificationToken
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

        [Column("used")]
        public bool Used { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
