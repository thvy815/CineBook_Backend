namespace ShowtimeService.Application.DTOs
{
    namespace ShowtimeService.Application.DTOs
    {
        public class ShowtimeDto
        {
            public Guid Id { get; set; }

            public Guid MovieId { get; set; }
            public string MovieTitle { get; set; } = "";
            public string PosterUrl { get; set; } = "";

            public Guid TheaterId { get; set; }
            public string TheaterName { get; set; } = "";
            public string TheaterAddress { get; set; } = "";

            public Guid RoomId { get; set; }

            public DateTime StartTime { get; set; }
            public string StartTimeFormatted { get; set; } = "";

            public string Date { get; set; } = ""; // yyyy-MM-dd
        }
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
