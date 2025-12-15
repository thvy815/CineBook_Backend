using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Domain.DTOs;
using System.Net.Http;

namespace PaymentService.Application.Services;

public class PaymentBusiness
{
    private readonly PaymentRepository _repo;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public PaymentBusiness(
        PaymentRepository repo,
        IConfiguration config,
        IHttpClientFactory httpFactory)
    {
        _repo = repo;
        _config = config;
        _http = httpFactory.CreateClient();
    }

    // ===================== CREATE ZALOPAY ORDER =====================
    public async Task<(string orderUrl, string appTransId)> CreateZaloPayOrder(
        Guid bookingId,
        Guid userId,
        decimal amountVnd)
    {
        // ===== CONFIG =====
        string appId = _config["ZaloPay:AppId"]!;
        string key1 = _config["ZaloPay:Key1"]!;
        string endpoint = _config["ZaloPay:Endpoint"]!;
        string callbackUrl = _config["ZaloPay:CallbackUrl"]!;
        string redirectUrl = _config["ZaloPay:RedirectUrl"]!;

        // ===== TIME (GMT+7 VIETNAM) =====
        var vnTime = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(7));
        string appTransId = vnTime.ToString("yyMMdd") + "_" + Guid.NewGuid().ToString("N")[..8];
        long appTime = vnTime.ToUnixTimeMilliseconds();

        // ===== DATA =====
        string appUser = userId.ToString();
        long amount = (long)amountVnd; // VND (KHÔNG *1000)
        string embedData = "{}";
        string item = "[]";

        // ===== MAC INPUT (CHUẨN ZALOPAY) =====
        string macInput =
            $"{appId}|{appTransId}|{appUser}|{amount}|{appTime}|{embedData}|{item}";

        string mac = ComputeHmacSha256(key1, macInput);

        // ===== FORM DATA =====
        var form = new Dictionary<string, string>
        {
            ["app_id"] = appId,
            ["app_trans_id"] = appTransId,
            ["app_user"] = appUser,
            ["app_time"] = appTime.ToString(),
            ["amount"] = amount.ToString(),
            ["item"] = item,
            ["embed_data"] = embedData,
            ["description"] = $"CineBook - Thanh toán đơn {appTransId}",
            ["bank_code"] = "zalopayapp",
            ["callback_url"] = callbackUrl,
            ["redirect_url"] = redirectUrl,
            ["mac"] = mac
        };

        var response = await _http.PostAsync(endpoint, new FormUrlEncodedContent(form));
        var json = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ZaloPayCreateOrderResponse>(json);

        if (result == null || result.return_code != 1)
        {
            throw new Exception($"ZaloPay create order failed: {json}");
        }

        // ===== SAVE DB =====
        await _repo.CreateAsync(new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            UserId = userId,
            Amount = amountVnd,
            Status = "PENDING",
            PaymentMethod = "ZaloPay",
            TransactionId = appTransId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        return (result.order_url, appTransId);
    }

    // ===================== CALLBACK =====================
    public async Task HandleCallback(ZaloPayCallbackRequest req)
    {
        Console.WriteLine("🔥 ZALOPAY CALLBACK RECEIVED");

        string key2 = _config["ZaloPay:Key2"]!;

        // 1. Verify MAC: dùng đúng data gốc

        // 2. Parse data
        var data = JsonConvert.DeserializeObject<ZaloPayCallbackData>(req.Data);
        if (data == null)
        {
            Console.WriteLine("❌ Data null");
            return;
        }

        // 3. Find payment
        var payment = await _repo.GetByTransactionRefAsync(data.app_trans_id);
        if (payment == null)
        {
            Console.WriteLine("❌ Payment not found");
            return;
        }

        // 4. Idempotent
        if (payment.Status == "SUCCESS")
        {
            Console.WriteLine("⚠️ Already SUCCESS");
            return;
        }

        // 5. SUCCESS LOGIC
        payment.Status = req.Type == 1 ? "SUCCESS" : "FAILED";
        payment.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(payment);
        

        Console.WriteLine($"✅ Payment updated: {payment.Status}");
    }



    // ===================== HMAC =====================
    private static string ComputeHmacSha256(string key, string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
