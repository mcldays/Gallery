using Gallery.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Utilits
{
    class EmailManager
    {
        private const string defaultError = "Ошибка при отправке сообщения!";
        private MailModel MailModel;

        public EmailManager()
        {
            MailModel = Explorer.MailModel;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SendEmail(string Email, string Path)
        {
            if (MailModel == null) return defaultError; 
            return await SendMail(MailModel.SMTPServer, MailModel.Port, MailModel.Mail, MailModel.Password, Email, MailModel.Title, "", Path);
        }

        public async static Task<string> SendMail(string smtpServer, int Port, string from, string password, string mailto, string caption, string message, string attachFile = null)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from);
                mail.To.Add(new MailAddress(mailto));
                mail.Subject = caption;
                mail.Body = message;
                if (!string.IsNullOrEmpty(attachFile))
                {
                    mail.Attachments.Add(new Attachment(attachFile));
                }

                SmtpClient client = new SmtpClient();
                client.Host = smtpServer;
                client.Port = Port;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(from.Split('@')[0], password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 1000000;
                await client.SendMailAsync(mail);
                mail.Dispose();
                return null;
            }
            catch (Exception e)
            {
                if(e.Message.Contains("limit"))
                {
                    return "Ошибка! Достигнут лимит сообщений для данного адреса. Укажите другой email, либо попробуйте позже.";
                }
                else if (e.Message.Contains("file too big"))
                {
                    return "Ошибка! Отправляемый файл превышает максимальный размер (30 MB)";
                }
                else 
                {
                    return defaultError;
                }
            }
        }
    }
}
