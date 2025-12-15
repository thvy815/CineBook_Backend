using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
[ApiController]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly PricingBusiness _business;

    public PricingController(PricingBusiness business)
    {
        _business = business;
    }

    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate(CalculatePriceRequest req)
    {
        var result = await _business.CalculateAsync(req);
        return Ok(result);
    }

    [HttpGet("fnb-items")]
    public async Task<IActionResult> GetFnbItems()
    {
        var items = await _business.GetAllFnbItemsAsync();
        return Ok(items);
    }

    [HttpGet("seat-prices")]
    public async Task<IActionResult> GetSeatPrices()
    {
        var prices = await _business.GetAllSeatPricesAsync();
        return Ok(prices);
    }

    [HttpGet("promotions")]
    public async Task<IActionResult> GetActivePromotions()
    {
        var items = await _business.GetActivePromotions();
        return Ok(items);
    }


}

