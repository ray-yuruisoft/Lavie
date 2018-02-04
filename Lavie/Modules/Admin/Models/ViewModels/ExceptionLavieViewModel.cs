using System;

namespace Lavie.Modules.Admin.Models.ViewModels
{
    public class ExceptionLavieViewModel : LavieViewModel
    {
        public ExceptionLavieViewModel(LavieViewModel model, Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}
