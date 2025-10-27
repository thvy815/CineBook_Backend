using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.DTOs
{
    public class BookingDTOs
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShowtimeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Version { get; set; }

        public List<BookingSeatDTOs>? BookingSeats { get; set; }
        public List<BookingFnbDTOs>? BookingFnbs { get; set; }
        public List<BookingPromotionDTOs>? BookingPromotions { get; set; }
 
    }
}
