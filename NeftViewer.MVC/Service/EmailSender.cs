using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using NeftViewer.MVC.Options;

namespace NeftViewer.MVC.Service
{
    public class EmailSender: IEmailSender
    {


        private readonly IOptions<SmtpParam> _smtpParam;
        public EmailSender(IOptions<SmtpParam> smtpParam)
        {
            _smtpParam = smtpParam;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_smtpParam.Value.Name, _smtpParam.Value.Sender));
            emailMessage.To.Add(new MailboxAddress(_smtpParam.Value.Sender, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_smtpParam.Value.Server, _smtpParam.Value.Port, SecureSocketOptions.None);
                if (_smtpParam.Value.Password!="")
                {
                    await client.AuthenticateAsync(_smtpParam.Value.Sender, _smtpParam.Value.Password);
                }
                
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }


    }
}
