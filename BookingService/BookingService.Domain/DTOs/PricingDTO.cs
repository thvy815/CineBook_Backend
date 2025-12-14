public class PricingSeatDto
{
    public string SeatType { get; set; } = null!;
    public string TicketType { get; set; } = null!;
    public int Quantity { get; set; }
}

public class PricingFnbDto
{
    public Guid FnbItemId { get; set; }
    public string Name { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class PricingCalculateRequest
{
    public List<PricingSeatDto> Seats { get; set; } = new();
    public List<PricingFnbDto> Fnbs { get; set; } = new();
    public PromotionDetailDto? Promotion { get; set; }
}

public class PromotionDetailDto
{
    public string Code { get; set; } = null!;
    public string DiscountType { get; set; } = null!;
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsOneTimeUse { get; set; }
    public string? Description { get; set; }
}


public class PricingCalculateResponse
{
    public decimal SeatTotal { get; set; }
    public decimal FnbTotal { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal FinalTotal { get; set; }

    public List<PricingFnbDto> Fnbs { get; set; } = new();
    public PromotionDetailDto? Promotion { get; set; }
}
