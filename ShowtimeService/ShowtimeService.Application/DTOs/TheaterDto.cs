namespace ShowtimeService.Application.DTOs
{
    public class TheaterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public Guid ProvinceId { get; set; }
        public string Status { get; set; }

    }

    public class CreateTheaterDto
    {
        public Guid ProvinceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

    public class AutoCreateTheaterRequest
    {
        public Guid ProvinceId { get; set; }
        public int NumberOfTheaters { get; set; }
    }
}
