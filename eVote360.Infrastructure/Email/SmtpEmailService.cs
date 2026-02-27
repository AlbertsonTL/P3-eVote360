using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using eVote360.Application.Abstractions.Emails;
using eVote360.Shared.Emails;

namespace eVote360.Infrastructure.Email;

public class SmtpEmailService : IEmailSender
{
    private readonly EmailSenderOptions _options;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSenderOptions> options, ILogger<SmtpEmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlMessage)
    {
        try
        {
            _logger.LogInformation("Enviando correo a {To} con asunto '{Subject}'", to, subject);

            using var client = new SmtpClient(_options.SmtpServer, _options.SmtpPort)
            {
                EnableSsl = _options.EnableSsl,
                Credentials = new NetworkCredential(_options.SenderEmail, _options.SenderPassword)
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_options.SenderEmail, "eVote360"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Correo enviado exitosamente a {To}", to);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "Error SMTP al enviar correo a {To}. Servidor: {Server}:{Port}, SSL: {SSL}",
                to, _options.SmtpServer, _options.SmtpPort, _options.EnableSsl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al enviar correo a {To}", to);
        }
    }
}
