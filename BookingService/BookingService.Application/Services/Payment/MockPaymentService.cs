namespace BookingService.Application.Services.Payment
{
    public class MockPaymentService : IPaymentService
    {
        public async Task<(bool success, string transactionId, string paymentMethod)> ChargeAsync(
            Guid userId, decimal amount, string method)
        {
            // Giả lập thanh toán thành công
            await Task.Delay(200);
            return (true, Guid.NewGuid().ToString(), method ?? "MockPay");
        }

        public async Task<bool> RefundAsync(string transactionId)
        {
            return await Task.FromResult(true);
        }
    }
}
