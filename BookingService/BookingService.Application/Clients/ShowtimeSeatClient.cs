using System.Net.Http.Json;

namespace BookingService.Application.Clients;

public class ShowtimeSeatClient
{
    private readonly HttpClient _http;
    public ShowtimeSeatClient(HttpClient http) => _http = http;

    public async Task<bool> LockSeat(Guid showtimeId, Guid seatId, string userId)
    {
        var res = await _http.PostAsync(
            $"/api/ShowtimeSeat/{showtimeId}/{seatId}/lock?userId={userId}", null);
        return res.IsSuccessStatusCode;
    }

    public async Task ReleaseSeat(Guid showtimeId, Guid seatId, string userId)
    {
        await _http.PostAsync(
            $"/api/ShowtimeSeat/{showtimeId}/{seatId}/release?userId={userId}", null);
    }

    public async Task BookSeats(Guid showtimeId, List<Guid> seatIds)
    {
        await _http.PostAsJsonAsync(
            $"/api/ShowtimeSeat/{showtimeId}/book", seatIds);
    }
}
