namespace ShowtimeService.Application.DTOs
{
    public class ShowtimeDto
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public Guid TheaterId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class CreateShowtimeDto
    {
        public Guid MovieId { get; set; }
        public Guid TheaterId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
