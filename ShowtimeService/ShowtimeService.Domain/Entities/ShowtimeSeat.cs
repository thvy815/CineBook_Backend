using System;

namespace ShowtimeService.Domain.Entities
{
    public class ShowtimeSeat
    {
        public Guid Id { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Showtime Showtime { get; set; }
        public Seat Seat { get; set; }
    }
}
