namespace BookingService.Application.Services.Payment
{
    public interface IPaymentService
    {
        Task<(bool success, string transactionId, string paymentMethod)> ChargeAsync(
            Guid userId, decimal amount, string method);

        Task<bool> RefundAsync(string transactionId);
    }
}
