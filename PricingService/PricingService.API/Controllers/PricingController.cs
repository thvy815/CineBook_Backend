using Microsoft.AspNetCore.Mvc;
using PricingService.Application.Services;
using PricingService.Domain.DTOs;
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

    // 🔹 GET: api/Promotion
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _business.GetAllPromotionsAsync();
        return Ok(data);
    }

    // 🔹 PUT: api/Promotion/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePromotionRequest request)
    {
        var result = await _business.UpdatePromotionAsync(id, request);
        if (result == null) return NotFound("Promotion not found");

        return Ok(result);
    }
}

