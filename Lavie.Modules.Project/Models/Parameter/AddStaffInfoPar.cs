using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.ModelValidation.Attributes;
using Lavie.Modules.Project.Extentions;

namespace Lavie.Modules.Project.Models
{
    public class AddStaffInfoPar
    {
        
        public int RiderEleID { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "请填写员工姓名")]
        [DisplayName("姓名")]
        public string Name { get; set; }

        [EnumRange(new int[] { 1, 2 }, ErrorMessage = "性别不正确")]
        [Required(ErrorMessage = "性别不能为空")]
        [DisplayName("性别")]
        public Int32 Sex { get; set; }

        [Range(16, 200)]
        [DisplayName("年龄")]
        public Int32? Age { get; set; }

        [DisplayName("出生日期")]
        [BirthDayModelBinder]
        public DateTime? Birthday { get; set; }

        [StringLength(20)]
        [Mobile(ErrorMessage = "手机号码格式不正确")]
        [DisplayName("手机号码")]
        [Required(ErrorMessage = "员工手机号码不能为空")]
        public string StaffMobile { get; set; }

        public MaritalStatus? MaritalStatus { get; set; }

        [RegularExpression(@"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", ErrorMessage = "身份证号码格式不正确")]
        public string IDCardNO { get; set; }

        public Int32? EducationID { get; set; }

        [StringLength(100)]
        public string School { get; set; }

        [StringLength(20)]
        public string Major { get; set; }

        [StringLength(20)]
        public string Household { get; set; }

        [StringLength(50)]
        public string Residence { get; set; }

        [StringLength(20)]
        public string EmergencyContact { get; set; }

        [StringLength(20)]
        public string EmergencyContactRelationship { get; set; }

        [StringLength(20)]
        [Mobile(ErrorMessage = "手机号码格式不正确")]
        public string EmergencyContactMobile { get; set; }

        [DisplayName("骑手职位类型ID")]
        public Int32? RiderJobTypeID { get; set; }
    }



}
