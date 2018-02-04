using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Lavie.Extensions;
using Lavie.Infrastructure;
using Lavie.Modules.Mail.Models;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.SmtpMail
{
    public class SmtpMailModule : Module, ISmtpMailModule
    {
        #region IModule Members

        public override string ModuleName
        {
            get { return "Mail"; }
        }

        #endregion

        #region ISmtpMailModule Members

        public void SendMail(string to, string subject, string body)
        {
            SendMail(new MessageOutbound
            {
                To = to,
                Subject = subject,
                Body = body
            });
        }

        public void SendMail(MessageOutbound message)
        {
            Guard.ArgumentNotNull(message, "message");

            SmtpClient smtpClient = GetSmtpClient();
            try
            {
                MailMessage messageToSend = new MailMessage(Settings.GetEmail("FromEmailAddress"), message.To)
                {
                    Subject = message.Subject,
                    Body = message.Body,
                    IsBodyHtml = true,
                    SubjectEncoding = System.Text.Encoding.UTF8,
                    BodyEncoding = System.Text.Encoding.UTF8
                };
                smtpClient.Send(messageToSend);
                message.MarkAsCompleted();
            }
            catch (Exception e)
            {
                message.MarkAsFailed(e);
            }
        }

        public void SendMails(IEnumerable<MessageOutbound> messages)
        {
            Guard.ArgumentNotNull(messages, "messages");

            foreach (var message in messages)
            {
                SendMail(message);
            }
        }

        #endregion

        #region Private Methods

        private SmtpClient GetSmtpClient()
        {
            SmtpClient client = new SmtpClient();

            client.Host = Settings.GetString("SmtpClient.Host");
            client.Port = Settings.GetInt32("SmtpClient.Port", 25);
            client.UseDefaultCredentials = Settings.GetBoolean("SmtpClient.UseDefaultCredentials", true);
            client.EnableSsl = Settings.GetBoolean("SmtpClient.EnableSsl", false);
            client.Timeout = Settings.GetInt32("SmtpClient.Timeout", 10000);

            string username = Settings.GetString("SmtpClient.Credentials.Username");
            string password = Settings.GetString("SmtpClient.Credentials.Password");
            string domain = Settings.GetString("SmtpClient.Credentials.Domain");

            if (!username.IsNullOrWhiteSpace() && !password.IsNullOrWhiteSpace())
            {
                //设置Credentials属性为NetworkCredential类型的值后，UseDefaultCredentials则为false
                client.Credentials = !domain.IsNullOrWhiteSpace()
                    ? new NetworkCredential(username, password, domain)
                    : new NetworkCredential(username, password);
            }
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            return client;
        }

        #endregion
    }
}
