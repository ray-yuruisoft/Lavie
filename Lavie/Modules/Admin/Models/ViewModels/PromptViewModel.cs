using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Lavie.Extensions;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Admin.Models.ViewModels
{
    public class PromptViewModel
    {
        private string _returnText;
        private string _returnUrl;

        public string Title { get; set; }
        public List<string> Message { get; set; }
        public string ReturnText
        {
            get { if (_returnText.IsNullOrWhiteSpace()) _returnText = "返回上一页"; return _returnText; }
            set { _returnText = value; }
        }
        public string ReturnURL
        {
            get { if (_returnUrl.IsNullOrWhiteSpace()) _returnUrl = "javascript:history.back();"; return _returnUrl; }
            set { _returnUrl = value; }
        }
        public PromptViewModel(string title)
            : this(title, null)
        {
            Title = title;
        }
        public PromptViewModel(string title, IEnumerable<String> message)
        {
            Title = title;
            Message = message == null ? new List<String>() : new List<String>(message);
        }
        public PromptViewModel(string title, IEnumerable<String> message, string returnText, string returnUrl)
            : this(title, message)
        {
            ReturnText = returnText;
            ReturnURL = returnUrl;
        }
        public void AddMessage(string message)
        {
            if(!message.IsNullOrWhiteSpace())
                Message.Add(message);
        }
        public void AddMessage(string format, params object[] args)
        {
            Message.Add(args.Length > 0 ? format.FormatWith(args) : format);
        }
        public void AddMessages(IEnumerable<String> message)
        {
            Guard.ArgumentNotNull(message, "message");

            Message.AddRange(message);
        }

        public LavieViewModelItem<PromptViewModel> LavieViewModelItem()
        {
            return  new LavieViewModelItem<PromptViewModel>(this);
        }
    }
}
