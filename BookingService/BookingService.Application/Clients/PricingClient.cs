using System.Net.Http.Json;

public class PricingClient
{
    private readonly HttpClient _http;

    public PricingClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PricingCalculateResponse> CalculateAsync(PricingCalculateRequest req)
    {
        var res = await _http.PostAsJsonAsync("/api/pricing/calculate", req);
        res.EnsureSuccessStatusCode();

        var price = await res.Content.ReadFromJsonAsync<PricingCalculateResponse>();
        if (price == null) throw new Exception("Pricing response is null");
        return price;
    }

}
