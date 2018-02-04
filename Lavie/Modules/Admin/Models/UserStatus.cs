using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Lavie.Modules.Admin.Models
{
    public enum UserStatus
    {
        [Display(Name = "未设")]
        NotSet = 0,
        [Display(Name = "正常")]
        Normal = 1,
        [Display(Name = "待审")]
        PendingApproval = 2,
        [Display(Name = "待删")]
        Removed = 3
    }
}
