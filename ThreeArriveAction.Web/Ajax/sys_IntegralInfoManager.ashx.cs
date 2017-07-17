using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_IntegralInfoManager 的摘要说明
    /// </summary>
    public class sys_IntegralInfoManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "integralInfoList")
            {
                IntegralInfoList(context);
            }
            else
            {
                context.Response.Write("错误的请求");
            }
        }


        public void IntegralInfoList(HttpContext context)
        {
            var userId = context.Request["userId"].ToInt();
            var page = context.Request["page"].ToInt();
            var size = context.Request["size"].ToInt();
            var sh = new SqlHelper<sys_IntegralInfoModel>(new sys_IntegralInfoModel())
            {
                PageConfig = new PageConfig
                {
                    SortEnum = SortEnum.Desc,
                    PageSortField = "IntegralDate",
                    PageSize = size,
                    PageIndex = page
                }
            };
            sh.AddWhere("UserId", userId);
            context.Response.Write(new
            {
                list = sh.Select().Select(x => new
                {
                    score = x.Score,
                    time = x.IntegralDate.ToString("yyyy-M-d"),
                    type = GetScoreTypeName(x.IntegralType)
                }),
                page,
                totle = sh.Total
            }.ToJson());
        }

        public string GetScoreTypeName(int type)
        {
            switch (type)
            {
                case 1: return "每日早报道";
                case 2: return "每周家家到";
                case 3: return "有事马上到";
                default: return "错误数据";
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}