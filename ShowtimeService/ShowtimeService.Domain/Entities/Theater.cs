using System;
using System.Collections.Generic;

namespace ShowtimeService.Domain.Entities
{
    public class Theater
    {
        public Guid Id { get; set; }
        public Guid ProvinceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Active";


        // Navigation properties
        public Province Province { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Showtime> Showtimes { get; set; }
    }
}
