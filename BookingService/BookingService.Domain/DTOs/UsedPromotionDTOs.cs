using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.DTOs
{
    public class UsedPromotionDTOs
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PromotionCode { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public DateTime UsedAt { get; set; }
    }
}
