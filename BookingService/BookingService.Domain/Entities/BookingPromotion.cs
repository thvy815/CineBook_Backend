using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Entities
{
    public class BookingPromotion
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string PromotionCode { get; set; } = "";
        public string DiscountType { get; set; } = "";
        public decimal DiscountValue { get; set; }

        // Navigation property
        public Booking? Booking { get; set; }
    }
}
