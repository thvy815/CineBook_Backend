using System;
using System.Collections.Generic;

namespace ShowtimeService.Domain.Entities
{
    public class Showtime
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid TheaterId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation properties
        public Theater Theater { get; set; }
        public Room Room { get; set; }
        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();
    }
}
