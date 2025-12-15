namespace ShowtimeService.Application.DTOs
{
    public class ShowtimeSeatDto
    {
        public Guid Id { get; set; }
        public Guid ShowtimeId { get; set; }
        public Guid SeatId { get; set; }

        public string SeatNumber { get; set; } = null!;
        public string RowLabel { get; set; } = null!;
        public int ColumnIndex { get; set; }

        public string SeatType { get; set; } = null!;
        public string Status { get; set; } = null!;
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