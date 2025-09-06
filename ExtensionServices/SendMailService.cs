using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Soil_Monitoring_Web_App.Services
{

    // Cấu hình dịch vụ gửi mail, giá trị Inject từ appsettings.json
    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }

    }

    // Dịch vụ gửi mail
    public class SendMailService : IEmailSender
    {
        private readonly MailSettings mailSettings;

        private readonly ILogger<SendMailService> logger;

        // mailSetting được Inject qua dịch vụ hệ thống
        // Có inject Logger để xuất log
        public SendMailService(IOptions<MailSettings> _mailSettings, ILogger<SendMailService> _logger)
        {
            mailSettings = _mailSettings.Value;
            logger = _logger;
            logger.LogInformation("Create SendMailService");
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                logger.LogError("SendEmailAsync called with null or empty email address.");
                throw new ArgumentException("Email address cannot be null or empty", nameof(email));
            }

            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName ?? "", mailSettings.Mail ?? "");
            message.From.Add(new MailboxAddress(mailSettings.DisplayName ?? "", mailSettings.Mail ?? ""));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject ?? "";

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage ?? ""
            };
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // Save email if sending fails
                System.IO.Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);

                logger.LogInformation("Sending error, saved at - " + emailsavefile);
                logger.LogError(ex, "Error while sending email.");
            }

            smtp.Disconnect(true);
            logger.LogInformation("Sent mail to: " + email);
        }
    }
}