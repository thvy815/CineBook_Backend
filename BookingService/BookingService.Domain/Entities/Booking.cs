using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShowtimeId { get; set; }
        public string Status { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
        public string? TransactionId { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Version { get; set; }

        // Navigation properties
        public ICollection<BookingSeat>? BookingSeats { get; set; }
        public ICollection<BookingFnb>? BookingFnbs { get; set; }
        public ICollection<BookingPromotion>? BookingPromotions { get; set; }
        public ICollection<UsedPromotion>? UsedPromotions { get; set; }
    }
}
