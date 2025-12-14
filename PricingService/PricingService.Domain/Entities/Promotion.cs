public class Promotion
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string DiscountType { get; set; } = null!; // PERCENT | AMOUNT
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsOneTimeUse { get; set; }
    public string? Description { get; set; }
}
