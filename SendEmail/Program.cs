using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace SendEmail
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            NetworkCredential credential = new NetworkCredential();
            credential.UserName = Properties.Settings.Default.EmailAuthen;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.PasswordAuthen))
                credential.Password = Properties.Settings.Default.PasswordAuthen;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = Properties.Settings.Default.SmtpServer;
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = credential;
            smtpClient.EnableSsl = Properties.Settings.Default.Ssl;
            smtpClient.Port = Properties.Settings.Default.SmtpPort;

            MailMessage message = new MailMessage();
            //message.Sender = new MailAddress(Properties.Settings.Default.FromEmail);
            message.From = new MailAddress(Properties.Settings.Default.FromEmail);

            string[] toEmails = Properties.Settings.Default.ToEmail.Split(',');
            foreach (string toEmail in toEmails)
                message.To.Add(toEmail);

            if (!string.IsNullOrEmpty(Properties.Settings.Default.CcEmail))
            {
                string[] ccEmails = Properties.Settings.Default.CcEmail.Split(',');
                foreach (string ccEmail in ccEmails)
                    message.To.Add(ccEmail);
            }

            message.Subject = Properties.Settings.Default.Subject;
            //message.Priority = MailPriority.High;
            message.Body = Properties.Settings.Default.Body;
            message.IsBodyHtml = true;

            smtpClient.Send(message);
        }
    }
}