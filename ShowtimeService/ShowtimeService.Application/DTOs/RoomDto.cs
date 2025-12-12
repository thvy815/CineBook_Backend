namespace ShowtimeService.Application.DTOs
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public Guid TheaterId { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }
        public string Status { get; set; }

    }

    public class CreateRoomDto
    {
        public Guid TheaterId { get; set; }
        public string Name { get; set; }
        public int SeatCount { get; set; }
        public string Status { get; set; }
    }

    public class AutoCreateRoomDto
    {
        public Guid TheaterId { get; set; }
        public int NumberOfRooms { get; set; }
        public int DefaultSeatCount { get; set; } = 100; // số ghế mặc định
    }
}
