using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly AuthDbContext _context;

        public EmailVerificationTokenRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
        }

        public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
        {
            return await _context.EmailVerificationTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
