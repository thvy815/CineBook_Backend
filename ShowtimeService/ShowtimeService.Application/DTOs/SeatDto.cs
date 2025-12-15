namespace ShowtimeService.Application.DTOs
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

    public class CreateSeatsRequest
    {
        public Guid RoomId { get; set; }
        public int Rows { get; set; }       // số hàng
        public int Columns { get; set; }    // số cột
        public int DoubleSeats { get; set; } // số ghế đôi
    }
}
