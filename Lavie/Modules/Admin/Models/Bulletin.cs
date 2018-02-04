using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Lavie.ModelValidation.Attributes;
using System.Web.Mvc;

namespace Lavie.Modules.Admin.Models
{
    public class Bulletin
    {
        public Guid BulletinID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? PublishDate { get; set; }
        public bool IsShow { get; set; }

    }
}
