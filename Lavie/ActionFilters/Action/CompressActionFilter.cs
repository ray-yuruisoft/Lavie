using System.Web;
using System.Web.Mvc;
using Lavie.Models;
using System.IO.Compression;

namespace Lavie.ActionFilters.Action
{
    public class CompressActionFilter : IActionFilter
    {
        #region IActionFilter Members

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            // load encodings from header
            var encodings = new QValueList(request.Headers["Accept-Encoding"]);

            // get the types we can handle, can be accepted and in the defined client preference
            QValue preferred = encodings.FindPreferred("gzip", "deflate", "identity");

            // if none of the preferred values were found, but the
            // client can accept wildcard encodings, we'll default
            // to Gzip.
            if (preferred.IsEmpty && encodings.AcceptWildcard && encodings.Find("gzip").IsEmpty)
                preferred = new QValue("gzip");

            HttpResponseBase response = filterContext.HttpContext.Response;
            // handle the preferred encoding
            switch (preferred.Name)
            {
                case "gzip":
                    response.AppendHeader("Content-encoding", "gzip");
                    response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
                    break;

                case "deflate":
                    response.AppendHeader("Content-encoding", "deflate");
                    response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
                    break;

                case "identity":
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}