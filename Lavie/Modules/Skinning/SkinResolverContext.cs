using System.Web.Routing;
using Lavie.Utilities.Exceptions;

namespace Lavie.Modules.Skinning
{
    /// <summary>
    /// 皮肤解析器上下文
    /// </summary>
    public class SkinResolverContext
    {
        public SkinResolverContext(RequestContext requestContext, string skin)
        {
            Guard.ArgumentNotNull(requestContext, "requestContext");
            Guard.ArgumentNotNull(skin, "skin");

            this.RequestContext = requestContext;
            this.Skin = skin;
        }
        public RequestContext RequestContext { get; private set; }
        public string Skin { get; private set; }
    }
}
