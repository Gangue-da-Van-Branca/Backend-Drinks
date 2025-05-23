using System.Net;
using System.Net.Mail;
using EloDrinksAPI.DTOs.email;
using Microsoft.Extensions.Logging;

namespace EloDrinksAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailRequestDto request)
        {
            var smtpServer = Environment.GetEnvironmentVariable("EMAIL_SMTP_SERVER");
            if (string.IsNullOrWhiteSpace(smtpServer)) throw new ArgumentNullException(nameof(smtpServer));

            var portStr = Environment.GetEnvironmentVariable("EMAIL_PORT");
            if (string.IsNullOrWhiteSpace(portStr)) throw new ArgumentNullException(nameof(portStr));

            var username = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentNullException(nameof(password));

            var fromName = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME");
            if (string.IsNullOrWhiteSpace(fromName)) throw new ArgumentNullException(nameof(fromName));

            var fromAddress = Environment.GetEnvironmentVariable("EMAIL_FROM_ADDRESS");
            if (string.IsNullOrWhiteSpace(fromAddress)) throw new ArgumentNullException(nameof(fromAddress));

            if (!int.TryParse(portStr, out int port))
                throw new ArgumentException("EMAIL_PORT deve ser um número válido");

            _logger.LogDebug("Preparando envio de e-mail de {FromAddress} para {To}", fromAddress, request.To);

            try
            {
                using var client = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromAddress, fromName),
                    Subject = request.Subject,
                    Body = request.Body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(request.To);
                await client.SendMailAsync(mailMessage);

                _logger.LogInformation("E-mail enviado com sucesso para {To}", request.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail: {Message}", ex.Message);
                throw;
            }
        }
    }
}
