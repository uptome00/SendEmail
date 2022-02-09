using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;

namespace SendEmail
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<string> errorMessages = new List<string>();
            string errorMessage;

            errorMessage = StopService(Properties.Settings.Default.TomcatServiceName);
            if (!string.IsNullOrEmpty(errorMessage))
                errorMessages.Add(errorMessage);

            errorMessage = StopService(Properties.Settings.Default.ElasticSearchService);
            if (!string.IsNullOrEmpty(errorMessage))
                errorMessages.Add(errorMessage);

            errorMessage = StartService(Properties.Settings.Default.TomcatServiceName);
            if (!string.IsNullOrEmpty(errorMessage))
                errorMessages.Add(errorMessage);

            errorMessage = StartService(Properties.Settings.Default.ElasticSearchService);
            if (!string.IsNullOrEmpty(errorMessage))
                errorMessages.Add(errorMessage);

            if (errorMessages.Count > 0)
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
                        message.CC.Add(ccEmail);
                }

                message.Subject = Properties.Settings.Default.Subject;
                //message.Priority = MailPriority.High;
                message.Body = Properties.Settings.Default.Body;
                foreach (string item in errorMessages)
                {
                    message.Body += "<br/>" + item;
                }
                message.IsBodyHtml = true;
                smtpClient.Send(message);
            }
        }

        public static string StopService(string serviceName)
        {
            string errorMessage = "";
            try
            {
                TimeSpan timeout = TimeSpan.FromSeconds(Properties.Settings.Default.WaitForStatusTimeoutSecond);
                ServiceController service = new ServiceController(serviceName);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                errorMessage = "[" + serviceName + "] Stop: " + ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " " + ex.InnerException.Message;
            }

            return errorMessage;
        }

        public static string StartService(string serviceName)
        {
            string errorMessage = "";
            try
            {
                TimeSpan timeout = TimeSpan.FromSeconds(Properties.Settings.Default.WaitForStatusTimeoutSecond);
                ServiceController service = new ServiceController(serviceName);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                errorMessage = "[" + serviceName + "] Start: " + ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " " + ex.InnerException.Message;
            }

            return errorMessage;
        }
    }
}