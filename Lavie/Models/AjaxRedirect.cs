using System.Runtime.Serialization;

namespace Lavie.Models
{
    [DataContract]
    public class AjaxRedirect
    {
        public AjaxRedirect(string url)
        {
            this.Url = url;
        }

        [DataMember]
        public string Url { get; private set; }
    }
}
