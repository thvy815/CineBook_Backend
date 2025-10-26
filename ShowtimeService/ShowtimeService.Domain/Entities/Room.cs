using System;
using System.Collections.Generic;

namespace ShowtimeService.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public Guid TheaterId { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }

        // Navigation properties
        public Theater Theater { get; set; }
        public ICollection<Seat> Seats { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}
