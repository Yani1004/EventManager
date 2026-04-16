using System.Net;
using System.Net.Mail;
using EventManager.Models;
using EventManager.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace EventManager.Services
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings _settings;

		public EmailService(IOptions<EmailSettings> settings)
		{
			_settings = settings.Value;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
		{
			using var message = new MailMessage
			{
				From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
				Subject = subject,
				Body = body,
				IsBodyHtml = isHtml
			};

			message.To.Add(toEmail);

			using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.Port)
			{
				Credentials = new NetworkCredential(_settings.Username, _settings.Password),
				EnableSsl = _settings.UseSsl
			};

			await smtpClient.SendMailAsync(message);
		}
	}
}