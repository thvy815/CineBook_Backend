using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowtimeService.Application.DTOs
{
    public class ShowtimeLiteDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;           // yyyy-MM-dd
        public string StartTimeFormatted { get; set; } = string.Empty; // HH:mm
    }

}
