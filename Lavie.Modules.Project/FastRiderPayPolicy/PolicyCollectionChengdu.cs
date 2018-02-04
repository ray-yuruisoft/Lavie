using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lavie.Modules.Project.FastRiderPayPolicy.Chengdu;

namespace Lavie.Modules.Project.FastRiderPayPolicy
{
    public class PolicyCollectionChengdu : IPolicyCollection
    {
        public List<IPolicy> Prolicies
        {
            get { return _Prolicies; }
            set { _Prolicies = value; }
        }
        private List<IPolicy> _Prolicies = new List<IPolicy>();

        public PolicyCollectionChengdu()
        {
            // 添加本城市的政策    
            Prolicies.AddRange(new List<IPolicy>() {

                new BasicDeliveryFee(),
                new EarliesAndLatesSubsidy(),
                new Evaluate(),
                new Referral()

            });
        }
        public void Compute(ComputeData data)
        {
            Prolicies.ForEach(m => m.Compute(data));
        }
    }
}
