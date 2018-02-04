using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lavie.Modules.Project.Models
{
    public enum RecruitChannelEnum
    {
        [Display(Name = "内部转介绍")]
        InnerReferrer = 1,
        [Display(Name = "网络招聘")]
        RecruitmentNetwork = 2,
        [Display(Name = "人才市场")]
        TalentMarket = 3,
        [Display(Name = "站点招聘")]
        RecruitmentStation = 4,
        [Display(Name = "人力中介")]
        HeadHunter = 4,
        [Display(Name = "其他")]
        Other = 5
    }
}
