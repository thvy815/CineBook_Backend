using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces
{
    public interface IEmailVerificationService
    {
        Task<(bool Success, string Message)> VerifyEmailAsync(string token);
        Task<(bool Success, string Message)> ResendVerificationEmailAsync(string email);
    }
}
