using MailKit.Net.Smtp;
using MimeKit;
using System; 


namespace Services
{
    public class EmailService
    {

       
         public void SendEmail(string recipientEmail)
        {
            try{ 
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(name: "John Doe", address: "joe@inbox.test"));
            message.To.Add(new MailboxAddress(name: "Recipient", address: recipientEmail));
            message.Subject = "Test subject";

            message.Body = new TextPart("plain")
            {
                Text = "Test body"
            };

            using (var client = new SmtpClient())
            {
                client.Connect(host: "mail.inbox.example", port: 587, useSsl: true);
                client.Authenticate(userName: "test", password: "test");
                client.Send(message);
                client.Disconnect(quit: true);
            }
            }
        
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the email: " + ex.Message);
            }
        }
    }
}
