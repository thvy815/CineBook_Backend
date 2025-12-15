using System;

namespace ShowtimeService.Domain.Entities
{
    public class Seat
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string SeatNumber { get; set; }
        public string RowLabel { get; set; }
        public int ColumnIndex { get; set; }
        public string Type { get; set; }

        // Navigation properties
        public Room? Room { get; set; }
        public ICollection<ShowtimeSeat> ShowtimeSeats { get; set; } = new List<ShowtimeSeat>();
    }
}
