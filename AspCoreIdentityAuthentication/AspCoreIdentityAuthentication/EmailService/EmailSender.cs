using System.Net.Mail;
using System.Net;

namespace AspCoreIdentityAuthentication.EmailService
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient("smtp.office365.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("youremailaddress", "password");

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("youremailaddress");
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = message;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        await client.SendMailAsync(mailMessage);
                    }
                    catch (SmtpException ex)
                    {
                        // Handle the exception or log the error
                        Console.WriteLine("SMTP Exception: " + ex.Message);
                        throw; // Optional: Rethrow the exception to propagate it further
                    }
                }
            }
        }

    }
}
