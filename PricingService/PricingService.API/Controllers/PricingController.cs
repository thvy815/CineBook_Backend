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
}

