using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace NeftViewer.MVC.Service
{
    public class EmailSender: IEmailSender
    {
        public EmailSender()
        {

        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "nhp-neftviewer@beloil.by"));
            emailMessage.To.Add(new MailboxAddress("nhp-neftviewer@beloil.by", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("mail-relay.it.beloil.by", 25, SecureSocketOptions.None);
                //await client.AuthenticateAsync("nhp-neftviewer@beloil.by", "");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }


    }
}
