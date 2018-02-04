using System.Web.Mvc;
using System.ServiceModel.Syndication;
using System.Web.Script.Serialization;
using System.Xml;

namespace Lavie.ActionResults
{
    public class AtomActionResult : ActionResult
    {
        public SyndicationFeed Feed { get; set; }

        public AtomActionResult() { }

        public AtomActionResult(SyndicationFeed feed)
        {
            this.Feed = feed;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/atom+xml";

            var formatter = new Atom10FeedFormatter(this.Feed);

            using (XmlWriter writer = XmlWriter.Create(context.HttpContext.Response.Output))
            {
                formatter.WriteTo(writer);
            }
        }
    }
}