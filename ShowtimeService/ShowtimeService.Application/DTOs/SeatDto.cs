﻿namespace ShowtimeService.Application.DTOs
{
    public class SeatDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public string SeatNumber { get; set; }
        public string RowLabel { get; set; }
        public int ColumnIndex { get; set; }
        public string Type { get; set; }
    }

    public class CreateSeatDto
    {
        public Guid RoomId { get; set; }
        public string SeatNumber { get; set; }
        public string RowLabel { get; set; }
        public int ColumnIndex { get; set; }
        public string Type { get; set; }
    }
}
