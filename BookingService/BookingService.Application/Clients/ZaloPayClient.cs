using System.Net.Http.Json;
using System.Text.Json;

namespace BookingService.Application.Clients;

public class ZaloPayClient
{
    private readonly HttpClient _http;

    public ZaloPayClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<(string orderUrl, string transactionId)>
        CreateOrderAsync(Guid bookingId, Guid userId, decimal amount)
    {
        var url =
            $"/api/payment/zalopay/create" +
            $"?bookingId={bookingId}" +
            $"&userId={userId}" +
            $"&amount={amount}";

        var res = await _http.PostAsync(url, null);

        var raw = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode)
            throw new Exception("PaymentService error: " + raw);

        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;

        string orderUrl = root.GetProperty("orderUrl").GetString()!;
        string transactionId = root.GetProperty("transactionId").GetString()!;

        return (orderUrl, transactionId);
    }
}
