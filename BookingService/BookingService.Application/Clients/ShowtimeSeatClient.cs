using System.Net.Http.Json;

namespace BookingService.Application.Clients;

public class ShowtimeSeatClient
{
    private readonly HttpClient _http;
    public ShowtimeSeatClient(HttpClient http) => _http = http;

    public async Task<bool> LockSeat(Guid showtimeId, Guid seatId)
    {
        var url = "/api/ShowtimeSeat/lock";
        var request = new
        {
            ShowtimeId = showtimeId,
            SeatId = seatId
        };
        var res = await _http.PostAsJsonAsync(url, request);
        return res.IsSuccessStatusCode;
    }

    public async Task ReleaseSeat(Guid showtimeId, Guid seatId)
    {
        var url = "/api/ShowtimeSeat/release";
        var request = new
        {
            ShowtimeId = showtimeId,
            SeatId = seatId
        };
        await _http.PostAsJsonAsync(url, request);
    }

    public async Task BookSeats(Guid showtimeId, List<Guid> seatIds)
    {
        var url = "/api/ShowtimeSeat/book";
        var request = new
        {
            ShowtimeId = showtimeId,
            SeatIds = seatIds
        };
        await _http.PostAsJsonAsync(url, request);
    }
    public async Task<List<Guid>> GetLockedSeats(Guid showtimeId)
    {
        var url = $"/api/ShowtimeSeat/locked?showtimeId={showtimeId}";
        var res = await _http.GetFromJsonAsync<List<Guid>>(url);
        return res ?? new List<Guid>();
    }

}
