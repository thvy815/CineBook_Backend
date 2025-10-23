using AuthService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;
		public EmailService(ILogger<EmailService> logger) { _logger = logger; }

		public Task SendEmailAsync(string to, string subject, string body)
		{
			// TODO: integrate real email (SMTP / SendGrid)
			_logger.LogInformation("SendEmail to {to}: {subject} - {body}", to, subject, body);
			return Task.CompletedTask;
		}
	}
}
