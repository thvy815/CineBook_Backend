using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingService.Domain.DTOs
{
    public class BookingSeatDTOs
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid SeatId { get; set; }
        public string SeatType { get; set; } = string.Empty;
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
