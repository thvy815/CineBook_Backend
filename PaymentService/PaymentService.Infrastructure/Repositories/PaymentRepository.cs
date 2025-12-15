using Microsoft.EntityFrameworkCore;

namespace PaymentService.Infrastructure.Repositories;

public class PaymentRepository
{
    private readonly PaymentDbContext _db;

    public PaymentRepository(PaymentDbContext db)
    {
        _db = db;
    }

    // ================= CREATE =================
    public async Task CreateAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
    }

    // ================= UPDATE =================
    public async Task UpdateAsync(Payment payment)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync();
    }

    // ================= GET BY TRANSACTION REF =================
    public async Task<Payment?> GetByTransactionRefAsync(string appTransId)
    {
        return await _db.Payments
            .FirstOrDefaultAsync(x => x.TransactionId == appTransId);
    }
}
