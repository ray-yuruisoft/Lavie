using System;
using System.Linq;
using Lavie.Infrastructure;
using Lavie.Utilities.Exceptions;

namespace Lavie.BackgroundServices
{
    public class MailMessageService : IBackgroundService
    {
        private readonly ISmtpMailModule _smtpMailModule;
        public MailMessageService(IModuleRegistry moduleRegistry)
        {
            Guard.ArgumentNotNull(moduleRegistry, "moduleRegistry");

            _smtpMailModule = moduleRegistry.GetModules<ISmtpMailModule>().LastOrDefault();
        }

        #region IBackgroundService 成员

        public string ServiceName { get; set; }

        public AppSettingsHelper Settings { get; set; }

        public TimeSpan Interval { get; set; }

        public void Initialize()
        {
        }

        public void Run()
        {
            if (_smtpMailModule != null)
                _smtpMailModule.SendMail("alby@foxmail.com", "测试邮件", "测试邮件内容");
        }

        public void Unload()
        {
        }

        #endregion
    }
}
