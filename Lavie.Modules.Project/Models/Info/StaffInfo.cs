using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Models
{
    public class StaffInfo
    {
        ///<summary>
        /// 个人ID
        ///</summary>
        public int StaffID { get; set; }

        ///<summary>
        /// 姓名
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// 部门
        ///</summary>
        public GroupBaseInfo Group { get; set; }
        public List<GroupBaseInfo> Groups { get; set; }

        ///<summary>
        /// 岗位
        ///</summary>
        public RoleBaseInfo Role { get; set; }

        ///<summary>
        /// 招聘渠道
        ///</summary>
        public RecruitChannelBaseInfo RecruitChannel { get; set; }

        ///<summary>
        /// 性别
        ///</summary>
        public int? Sex { set; get; }

        ///<summary>
        /// 联系方式
        ///</summary>
        public string StaffMobile { get; set; }

        ///<summary>
        /// 入职时间
        ///</summary>
        public DateTime? EntryDate { get; set; }

        ///<summary>
        /// 类型
        ///</summary>
        public RiderJobTypeInfo RiderJobType { get; set; }

        ///<summary>
        /// 结束跑单日期
        ///</summary>
        public DateTime? RiderLastWorkDate { get; set; }

        ///<summary>
        /// 年龄
        ///</summary>
        public int? Age { get; set; }

        ///<summary>
        /// 婚姻状况
        ///</summary>
        
        public MaritalStatus? MaritalStatus { get; set; }


        ///<summary>
        /// 生日
        ///</summary>
        public DateTime? BirthDay { get; set; }

        ///<summary>
        /// 工龄
        ///</summary>
        //public int? Seniority { get; set; }

        ///<summary>
        /// 本人银行卡
        ///</summary>
        public string BankNO { get; set; }

        ///<summary>
        /// 开户行
        ///</summary>
        public BankTypeInfo BankType { get; set; }

        ///<summary>
        /// 学历
        ///</summary>
        public EducationInfo Education { get; set; }

        ///<summary>
        /// 毕业学校
        ///</summary>
        public string School { get; set; }

        ///<summary>
        /// 职业等级资格或专业
        ///</summary>
        public string Major { get; set; }

        ///<summary>
        /// 身份证
        ///</summary>
        public string IDCardNO { get; set; }

        ///<summary>
        /// 户籍所在地
        ///</summary>
        public string Household { get; set; }

        ///<summary>
        /// 现居地
        ///</summary>
        public string Residence { get; set; }

        ///<summary>
        /// 紧急联系人
        ///</summary>
        public string EmergencyContact { get; set; }

        ///<summary>
        /// 紧急联系人与本人关系
        ///</summary>
        public string EmergencyContactRelationship { get; set; }

        ///<summary>
        /// 紧急联系人手机号
        ///</summary>
        public string EmergencyContactMobile { get; set; }

        ///<summary>
        /// 用工性质ID
        ///</summary>
        public JobNatureInfo JobNature { get; set; }

        ///<summary>
        /// 协议编号
        ///</summary>
        public string ProtocolNO { get; set; }

        ///<summary>
        /// 合同期次
        ///</summary>
        public ProtocolTimeBaseInfo ProtocolTime { get; set; }

        ///<summary>
        /// 协议类型（合同类型）ID
        ///</summary>
        public ProtocolType ProtocolType { get; set; }

        ///<summary>
        /// 协议签署日期
        ///</summary>
        public System.DateTime? ProtocolSignedDate { get; set; }

        ///<summary>
        /// 协议开始日期
        ///</summary>
        public System.DateTime? ProtocolBeginDate { get; set; }

        ///<summary>
        /// 协议结束日期
        ///</summary>
        public System.DateTime? ProtocolEndDate { get; set; }

        ///<summary>
        /// 保险性质ID
        ///</summary>
        public InsuranceNatureBaseInfo InsuranceNature { get; set; }

        ///<summary>
        /// 异动类型
        ///</summary>
        public StaffStatusBaseInfo StaffStatus { get; set; }

        ///<summary>
        /// 保险开始购买日期
        ///</summary>
        public System.DateTime? InsuranceStartBuyDate { get; set; }

        ///<summary>
        /// 保险结束购买日期
        ///</summary>
        public System.DateTime? InsuranceStopBuyDate { get; set; }

        ///<summary>
        /// 入职备注
        ///</summary>
        public string EntryRemark { get; set; }

        ///<summary>
        /// 离职时间
        ///</summary>
        public System.DateTime? ExitDate { get; set; }

        ///<summary>
        /// 离职原因ID
        ///</summary>
        public ExitReasonBaseInfo ExitReason { get; set; }

        ///<summary>
        /// 离职备注
        ///</summary>
        public string ExitRemark { get; set; }

        public StaffInfo()
        {
            Groups = new List<GroupBaseInfo>();
        }
    }


    public class GroupBaseInfo
    {
        public Guid GroupID { get; set; }
        public string Name { get; set; }
    }
    public class RoleBaseInfo
    {
        public Guid? RoleID { get; set; }
        public string Name { get; set; }
    }
    public class StaffStatusBaseInfo
    {
        public int StaffStatusID { get; set; }
        public string Name { get; set; }
    }
    public class ProtocolTimeBaseInfo
    {
        public int? ProtocolTimeID { get; set; }
        public string Name { get; set; }
    }
    public class RecruitChannelBaseInfo
    {
        public int? RecruitChannelID { get; set; }
        public string Name { get; set; }
    }
    public class ProtocolType
    {
        public int? ProtocolTypeID { get; set; }
        public string Name { get; set; }
    }
    public class InsuranceNatureBaseInfo
    {
        public int? InsuranceNatureID { get; set; }
        public string Name { get; set; }
    }
    public class ExitReasonBaseInfo
    {
        public int? ExitReasonID { get; set; }
        public string Name { get; set; }
    }
    public class MaritalStatusBaseInfo
    {
        public MaritalStatus MaritalStatuID { get; set; }
        public string Name { get; set; }
    }


}
