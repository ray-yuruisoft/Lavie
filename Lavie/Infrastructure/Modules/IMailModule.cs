using System;
using System.Collections.Generic;
using Lavie.Models;
using Lavie.Modules.Mail.Models;

namespace Lavie.Infrastructure
{
    public interface ISmtpMailModule : IModule
    {
        void SendMail(string to, string subject,string body);
        void SendMail(MessageOutbound message);
        void SendMails(IEnumerable<MessageOutbound> messages);
    }
}
