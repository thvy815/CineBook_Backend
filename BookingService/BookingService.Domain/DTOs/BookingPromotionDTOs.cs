using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.DTOs
{
    public class BookingPromotionDTOs
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
    }
}
