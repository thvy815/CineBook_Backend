namespace BookingService.Application.Clients;

public class PricingClient
{
    public async Task<(decimal total, decimal discount, decimal final)> CalculatePrice()
    {
        await Task.Delay(50);
        return (200000, 20000, 180000);
    }
}
