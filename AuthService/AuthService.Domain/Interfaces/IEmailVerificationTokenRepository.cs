using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces
{
    public interface IEmailVerificationTokenRepository
    {
        Task AddAsync(EmailVerificationToken token);
        Task<EmailVerificationToken?> GetByTokenAsync(string token);
        Task SaveChangesAsync();
    }
}
