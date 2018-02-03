using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lavie.Environment
{
    public interface ILavieHost
    {
        void Initialize(HttpApplication application);
        void Disponse(HttpApplication application);
        void BeginRequest(HttpApplication application);
        void PostMapRequestHandler(HttpApplication application);
        void EndRequest(HttpApplication application);

    }
}
