public record CalculatePriceRequest(
    List<SeatRequest> Seats,
    List<FnbRequest> Fnbs,
    String? PromotionCode
);

public record SeatRequest(
    string SeatType,
    string TicketType,
    int Quantity
);

public record FnbRequest(
    Guid FnbItemId,
    string Name,
    decimal UnitPrice,
    int Quantity
);

public record CalculatePriceResponse(
    decimal[] SeatPrice,
    decimal FnbTotal,
    decimal SubTotal,
    decimal Discount,
    decimal FinalTotal,
    List<FnbRequest> Fnbs,
    PromotionDetailDto? Promotion
);

public record PromotionDetailDto(
    string Code,
    string DiscountType,
    decimal DiscountValue,
    decimal DiscountAmount,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool IsOneTimeUse,
    string? Description
);
