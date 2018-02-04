using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Admin.Models;

namespace Lavie.Modules.Project.Models
{
    public class StaffDictionary
    {
        public List<UserStatusInfoBase> UserStatus { get; set; }
        public List<EducationInfo> Educations { get; set; }
        public List<RecruitChannelInfo> RecruitChannels { get; set; }
        public List<JobNatureInfo> JobNatures { get; set; }
        public List<ProtocolTimeInfo> ProtocolTimes { get; set; }
        public List<ProtocolTypeInfo> ProtocolTypes { get; set; }
        public List<BankTypeInfo> BankTypes { get; set; }
        public List<InsuranceNatureInfo> InsuranceNatures { get; set; }
        public List<RiderJobTypeInfo> RiderJobTypes { get; set; }
    }
}
