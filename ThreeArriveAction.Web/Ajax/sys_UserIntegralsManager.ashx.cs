using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_UserIntegralsManager 的摘要说明
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class sys_UserIntegralsManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "integralList")
            {
                IntegralList(context);
            }
            else if (type == "getModel")
            {
                GetModel(context);
            }
            else
            {
                context.Response.Write("错误的请求");
            }
        }

        public void GetModel(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var sh = new SqlHelper<sys_UserIntegralsModel>(new sys_UserIntegralsModel());
            sh.AddWhere("IntegralId", id);
            var model = sh.Select().FirstOrDefault();
            var villageList = DataConfig.GetVillages();
            var user = DataConfig.GetUsers(new Dictionary<string, object>
                                           {
                                               {"UserId", model?.UserId}
                                           }).FirstOrDefault();
            var village = villageList.FirstOrDefault(x => x.VillageId == user?.VillageId);
            context.Response.Write(new
            {
                name = user?.UserName,
                village = (villageList.FirstOrDefault(x => x.VillageId == village?.VillageParId)?.VillageName ?? "淮安") + "--" + village?.VillageName,
                score = model?.IntegralScore,
                userId = user?.UserId
            }.ToJson());
        }

        public void IntegralList(HttpContext context)
        {
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var ddlvillage = context.Request["ddlvillage"].ToInt();

            var sh = new SqlHelper<sys_UserIntegrals>(new sys_UserIntegrals())
            {
                PageConfig = new PageConfig
                {
                    PageIndex = page,
                    PageSize = size,
                    PageSortField = "i.IntegralScore",
                    SortEnum = SortEnum.Desc
                }
            };
            sh.AddShow("u.UserName,u.VillageId,i.IntegralScore,i.IntegralId");
            sh.Alia = "i";
            sh.AddJoin(JoinEnum.LeftJoin, "sys_Users", "u", "UserId", "UserId");
            sh.AddWhere("IntegralYear", DateTime.Now.Year);
            sh.AddWhere("IntegralMonth", DateTime.Now.Month);
            if (ddlvillage > 0)
            {
                sh.AddWhere("u.VillageId", ddlvillage);
            }

            var villageList = DataConfig.GetVillages();
            var list = new ArrayList();

            foreach (var model in sh.Select())
            {
                var village = villageList.FirstOrDefault(x => x.VillageId == model.VillageId);
                list.Add(new
                {
                    id = model.IntegralId,
                    name = model.UserName,
                    village = (villageList.FirstOrDefault(x => x.VillageId == village?.VillageParId)?.VillageName ?? "淮安") + "--" + village?.VillageName,
                    integra = model.IntegralScore
                });
            }

            context.Response.Write(new
            {
                list,
                totle = sh.Total,
                page,

            }.ToJson());
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        class sys_UserIntegrals
        {
            public string UserName;
            public int VillageId;
            public int IntegralScore;
            public int IntegralId;
        }
    }
}