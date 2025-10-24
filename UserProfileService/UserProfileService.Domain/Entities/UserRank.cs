using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileService.Domain.Entities
{
	[Table("user_ranks")]
	public class UserRank
	{
		[Key]
		[Column("id")]
		public Guid Id { get; set; }

		[Column("name")]
		[MaxLength(50)]
		public string Name { get; set; }

		[Column("min_points")]
		public int MinPoints { get; set; }

		[Column("max_points")]
		public int MaxPoints { get; set; }

		[Column("discount_rate")]
		public decimal DiscountRate { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime? UpdatedAt { get; set; }
	}
}
