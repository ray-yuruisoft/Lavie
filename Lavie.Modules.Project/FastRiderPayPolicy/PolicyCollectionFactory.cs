using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lavie.Extensions;
using Lavie.Modules.Admin.Services;
using Lavie.Modules.Project.FastRiderPayPolicy.Chengdu;

namespace Lavie.Modules.Project.FastRiderPayPolicy
{
    /// <summary>
    /// 政策
    /// </summary>
    public interface IPolicy
    {
        void Compute(ComputeData data);

    }
    /// <summary>
    /// 政策集
    /// </summary>
    public interface IPolicyCollection
    {
        List<IPolicy> Prolicies {get; set;}
        void Compute(ComputeData data);
    }

    public static class PolicyCollectionFactory
    {
        // TODO: 后期改为返回树状的政策集，那需要考虑区别同类政策，以及同类政策从下至上的优先级
        public static async Task<IPolicyCollection> CreatePolicyByStationID(Guid stationID)
        {
            var groupService = DependencyResolver.Current.GetService<IGroupService>();
            if (groupService == null)
            {
                throw new Exception("内部错误：无法获取 IGroupService");
            };

            var groupPath = await groupService.GetInfoPathAsync(stationID);
            if (groupPath == null || groupPath.Count != 5)
            {
                throw new Exception("内部错误：stationID 对应的部门可能不是站点");
            }

            Guid cityID = groupPath[2].GroupID;
            // TODO: 当前总是返回新实例，如果合适也可缓存起来。
            // 由于 Guid 不是基础数据类型，不能使用 switch; 由于 Guid 不好和类名对应，故不使用反射（可通过配置文件的方式来对应，目前不做）。
            if (cityID == new Guid("35dea0e5-9b9f-4b8b-981e-2e1120b1c317"))
            {
                // Chengdu
                return new PolicyCollectionChengdu();
            }
            else if (cityID == new Guid("8aa4516f-ae0c-4dc9-a463-85f8dba4b03e"))
            {
                // Hefei
            }
            else if (cityID == new Guid("28c9b6d8-dd00-4bc9-bd3e-e06174dfaaeb"))
            {
                // Lanzhou
            }
            else if (cityID == new Guid("e6881d44-c072-4f8d-831e-8cb3304b3ab9"))
            {
                // Xian
            }
            else if (cityID == new Guid("369691ef-3162-4ffa-ba73-80d6473ade91"))
            {
                // Zhengzhou
            }

            throw new NotImplementedException();
        }

        public static IPolicyCollection CreatePolicy(Guid cityID)
        {
            // TODO: 当前总是返回新实例，如果合适也可缓存起来。
            // 由于 Guid 不是基础数据类型，不能使用 switch; 由于 Guid 不好和类名对应，故不使用反射（可通过配置文件的方式来对应，目前不做）。
            if (cityID == new Guid("35dea0e5-9b9f-4b8b-981e-2e1120b1c317"))
            {
                // Chengdu
                return new PolicyCollectionChengdu();
            }
            else if (cityID == new Guid("8aa4516f-ae0c-4dc9-a463-85f8dba4b03e"))
            {
                // Hefei
            }
            else if (cityID == new Guid("28c9b6d8-dd00-4bc9-bd3e-e06174dfaaeb"))
            {
                // Lanzhou
            }
            else if (cityID == new Guid("e6881d44-c072-4f8d-831e-8cb3304b3ab9"))
            {
                // Xian
            }
            else if (cityID == new Guid("369691ef-3162-4ffa-ba73-80d6473ade91"))
            {
                // Zhengzhou
            }

            throw new NotImplementedException();
        }

    }

    public enum City
    {
        Chengdu,
        Hefei,
        Lanzhou,
        Xian,
        Zhengzhou
    }

}
