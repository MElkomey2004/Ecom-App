using Ecom.Core.DTO;
using Ecom.Core.services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories.Service
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmail(EmailDTO emailDTO)
		{
			if (string.IsNullOrWhiteSpace(emailDTO.To))
				throw new ArgumentException("Recipient email cannot be null or empty", nameof(emailDTO.To));

			var message = new MimeMessage();
			message.From.Add(new MailboxAddress("My Ecom", _configuration["SmtpSettings:From"]));
			message.To.Add(MailboxAddress.Parse(emailDTO.To));
			message.Subject = emailDTO.Subject ?? "(No Subject)";
			message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
			{
				Text = emailDTO.Content ?? string.Empty
			};

			using var smtp = new SmtpClient();
			try
			{
				await smtp.ConnectAsync(
					_configuration["SmtpSettings:Host"],
					int.Parse(_configuration["SmtpSettings:Port"]),
					SecureSocketOptions.StartTls
				);

				await smtp.AuthenticateAsync(
					_configuration["SmtpSettings:Username"],
					_configuration["SmtpSettings:Password"]
				);

				await smtp.SendAsync(message);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Failed to send email", ex);
			}
			finally
			{
				await smtp.DisconnectAsync(true);
			}
		}
	}
}
