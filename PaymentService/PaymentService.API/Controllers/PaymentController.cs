using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentService.Application.Services;
using PaymentService.Domain.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly PaymentBusiness _business;

    public PaymentController(PaymentBusiness business)
    {
        _business = business;
    }

    [HttpPost("zalopay/create")]
    public async Task<IActionResult> CreateZaloPayOrder(
        Guid bookingId,
        Guid userId,
        decimal amount)
    {
        var result = await _business.CreateZaloPayOrder(bookingId, userId, amount);
        return Ok(new
        {
            orderUrl = result.orderUrl,
            transactionId = result.appTransId
        });
    }

    [HttpPost("zalopay/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> ZaloPayCallback()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        Console.WriteLine("🔥 ZALOPAY CALLBACK RECEIVED");
        Console.WriteLine(body); // log payload raw

        // Parse JSON
        var wrapper = JsonConvert.DeserializeObject<ZaloPayCallbackRequest>(body);
        var data = JsonConvert.DeserializeObject<ZaloPayCallbackData>(wrapper.Data);

        // Xác định trạng thái
        var status = wrapper.Type == 1 ? "SUCCESS" : "FAILED";
        var transactionId = data.app_trans_id;
        var bookingId = Guid.Parse(data.app_user); // Hoặc mapping transactionId -> bookingId

        // 1️⃣ Cập nhật PaymentService record
        await _business.HandleCallback(wrapper);

        // 2️⃣ Gọi BookingService để tự động update booking
        using var client = new HttpClient();
        await client.PostAsJsonAsync(
            $"https://localhost:7023/api/bookings/api/payments/callback",
            new
            {
                BookingId = bookingId,
                Status = status,
                TransactionId = transactionId,
                PaymentMethod = "ZaloPay"
            }
        );

        return Ok(new { return_code = 1, return_message = "success" });
    }

}
