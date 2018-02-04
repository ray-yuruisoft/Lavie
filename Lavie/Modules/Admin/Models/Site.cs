using System;

namespace Lavie.Modules.Admin.Models
{
    public class Site
    {
        public Guid SiteID { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public string FavIconURL { get; set; }
        public string PageTitleSeparator { get; set; }
    }
}
