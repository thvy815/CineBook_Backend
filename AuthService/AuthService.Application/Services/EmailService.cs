using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
	public class EmailService : IEmailService
	{
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options) 
        { 
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
		{
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SenderEmail, _settings.Password),
                EnableSsl = true
            };

            var mail = new MailMessage()
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(to);

            await client.SendMailAsync(mail);
        }

        public async Task SendVerifyEmailAsync(string email, string token)
        {
            var link = $"https://localhost:7039/api/Auth/verify-email?token={token}";

            var body = $@"
                        <h3>Xác thực email</h3>
                        <p>Click link bên dưới để xác thực tài khoản:</p>
                        <a href='{link}'>{link}</a>
                        <p>Link có hiệu lực 30 phút</p>
                    ";

            await SendEmailAsync(email, "Xác thực email", body);
        }
    }
}
