using System;

namespace UserProfileService.Domain.DTOs
{
	public class UserRankDTO
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int MinPoints { get; set; }
		public int MaxPoints { get; set; }
		public decimal DiscountRate { get; set; }
	}
}
