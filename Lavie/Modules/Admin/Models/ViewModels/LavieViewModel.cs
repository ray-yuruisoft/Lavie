using System;
using System.Collections.Generic;
using System.Linq;
using Lavie.Models;
using Lavie.Infrastructure;

namespace Lavie.Modules.Admin.Models.ViewModels
{

    /// <summary>
    /// Main view model used in Lavie
    /// </summary>
    public class LavieViewModel
    {

        public static LavieViewModel Empty
        {
            get { return new LavieViewModel(); }
        }

        public Site Site { get; set; }
        public IUser User { get; set; }
        public IAuthenticationModule AuthenticationModule { get; set; }
        public string ReturnURL { get; set; }

    }
}
