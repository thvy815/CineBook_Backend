namespace ShowtimeService.Application.DTOs
{
    public class ProvinceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateProvinceDto
    {
        public string Name { get; set; }
    }
    public class UpdateProvinceDto
    {
        public string Name { get; set; } = null!;
    }
}
