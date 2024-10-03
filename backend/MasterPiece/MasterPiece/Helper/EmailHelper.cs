using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace MasterPiece.Helper
{
    public class EmailHelper
    {
        private readonly IConfiguration _configuration;

        public EmailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMessage(string name, string email, string subject, string message)
        {
            try
            {
                string toEmail = email;
                string smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                string smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                // Validate the inputs
                if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
                if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.");

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(email.Trim(), name.Trim());
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;  // Set subject here
                    mailMessage.Body = message.Trim();
                    mailMessage.IsBodyHtml = false;

                    using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                        smtpClient.EnableSsl = true;

                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (SmtpException smtpEx)
            {
                throw new InvalidOperationException("Error sending email via SMTP", smtpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while sending the email", ex);
            }
        }

    }
}
