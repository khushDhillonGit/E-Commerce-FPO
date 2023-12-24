using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Cms;
using System.Net.Mail;

namespace JattanaNursury.Services
{
    public class EmailSender : IEmailSender
    {

        private string _fromAddress;
        private string _username;
        private string _password;

        public EmailSender(IConfiguration configuration) 
        {
            _fromAddress = configuration["SenderEmail"];
            _username = configuration["GoogleUserName"];
            _password = configuration["GooglePassword"];
        }


        public async Task SendEmailAsync(string toEmail, string subject, string email)
        {
          

            //var gmailClient = new System.Net.Mail.SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    UseDefaultCredentials = false,
            //    Credentials = new System.Net.NetworkCredential(_username, _password)
            //};

            //using (var msg = new System.Net.Mail.MailMessage(_fromAddress,toEmail, subject, email))
            //{
                
            //    msg.To.Add(toEmail);

            //    try
            //    {
            //        gmailClient.Send(msg);
           
            //    }
            //    catch (Exception)
            //    {
            //        // TODO: Handle the exception
                
            //    }
            //}


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Admin", _fromAddress));
            message.To.Add(new MailboxAddress("Customer", toEmail));
            message.Subject = subject;

            var body = new MimeKit.BodyBuilder
            {
                HtmlBody = email
            };

            message.Body = body.ToMessageBody();



            using var client = new MailKit.Net.Smtp.SmtpClient();
            client.Connect("smtp.gmail.com", 465, true);
            client.Authenticate(_username, _password);

            await client.SendAsync(message);



            client.Disconnect(true);

        }
    }
}
