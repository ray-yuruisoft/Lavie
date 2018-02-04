using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lavie.Modules.Mail.Models
{
    public class MessageOutbound
    {
        public Guid ID { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public DateTime? Sent { get; set; }
        public Exception Error { get; set; }
        public int RemainingRetryCount { get; set; }

        public void MarkAsCompleted()
        {
            Sent = DateTime.UtcNow;
            RemainingRetryCount = 0;
        }

        public void MarkAsFailed(Exception error)
        {
            Sent = null;
            RemainingRetryCount--;
            Error = error;
        }
    }
}
