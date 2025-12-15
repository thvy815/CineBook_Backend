using ShowtimeService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowtimeService.Application.DTOs
{
    public class TheaterShowtimeDto
    {
        public Guid TheaterId { get; set; }
        public string TheaterName { get; set; } = "";
        public string TheaterAddress { get; set; } = "";

        public List<ShowtimeLiteDto> Showtimes { get; set; } = new();
    }
}