namespace ShowtimeService.Application.DTOs
{
    public class ShowtimeSeatDto
    {
        public Guid Id { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateShowtimeSeatDto
    {
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
