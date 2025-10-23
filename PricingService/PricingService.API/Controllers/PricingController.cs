using Microsoft.AspNetCore.Mvc;
using PricingService.Domain.Entities;
using PricingService.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace PricingService.API.Controllers
{
	public class PricingController
	{
        // ================================
        // 1) SEAT PRICE CONTROLLER
        // ================================
        [ApiController]
        [Route("api/[controller]/seatprice")]
        public class SeatPriceController : ControllerBase
        {
            private readonly SeatPriceBusiness _business;

            public SeatPriceController(SeatPriceBusiness business)
            {
                _business = business;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await _business.GetAllAsync();
                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var seatPrice = await _business.GetByIdAsync(id);
                if (seatPrice == null)
                    return NotFound();
                return Ok(seatPrice);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] SeatPrice seatPrice)
            {
                await _business.AddAsync(seatPrice);
                return CreatedAtAction(nameof(GetById), new { id = seatPrice.Id }, seatPrice);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] SeatPrice seatPrice)
            {
                if (id != seatPrice.Id)
                    return BadRequest("ID in URL does not match ID in body");

                await _business.UpdateAsync(seatPrice);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                await _business.DeleteAsync(id);
                return NoContent();
            }
        }

        // ================================
        // 2) FNB ITEM CONTROLLER
        // ================================
        [ApiController]
        [Route("api/[controller]/fnbitem")]
        public class FnbItemController : ControllerBase
        {
            private readonly FnbItemBusiness _business;

            public FnbItemController(FnbItemBusiness business)
            {
                _business = business;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await _business.GetAllAsync();
                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var item = await _business.GetByIdAsync(id);
                if (item == null)
                    return NotFound();
                return Ok(item);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] FnbItem fnbItem)
            {
                await _business.AddAsync(fnbItem);
                return CreatedAtAction(nameof(GetById), new { id = fnbItem.Id }, fnbItem);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] FnbItem fnbItem)
            {
                if (id != fnbItem.Id)
                    return BadRequest("ID in URL does not match ID in body");

                await _business.UpdateAsync(fnbItem);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                await _business.DeleteAsync(id);
                return NoContent();
            }
        }

        // ================================
        // 3) PROMOTION CONTROLLER
        // ================================
        [ApiController]
        [Route("api/[controller]/promotion")]
        public class PromotionController : ControllerBase
        {
            private readonly PromotionBusiness _business;

            public PromotionController(PromotionBusiness business)
            {
                _business = business;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var result = await _business.GetAllAsync();
                return Ok(result);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(Guid id)
            {
                var promo = await _business.GetByIdAsync(id);
                if (promo == null)
                    return NotFound();
                return Ok(promo);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] Promotion promotion)
            {
                await _business.AddAsync(promotion);
                return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(Guid id, [FromBody] Promotion promotion)
            {
                if (id != promotion.Id)
                    return BadRequest("ID in URL does not match ID in body");

                await _business.UpdateAsync(promotion);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(Guid id)
            {
                await _business.DeleteAsync(id);
                return NoContent();
            }
        }
    }
}
