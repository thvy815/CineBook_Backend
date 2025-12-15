namespace PaymentService.Domain.DTOs;
public class ZaloPayCallbackRequest
{
    public string? Data { get; set; }
    public string? Mac { get; set; }
    public int? Type { get; set; }
}

public class ZaloPayCallbackData
{
    public int app_id { get; set; }
    public string app_trans_id { get; set; } = "";
    public long app_time { get; set; }
    public string embed_data { get; set; }
    public string app_user { get; set; } = "";
    public long amount { get; set; }
    public string zp_trans_id { get; set; } = ""; // QUAN TRỌNG
}



