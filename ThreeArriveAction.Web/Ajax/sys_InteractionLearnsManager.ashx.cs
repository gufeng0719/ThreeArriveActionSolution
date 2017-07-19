using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_InteractionLearnsManager 的摘要说明
    /// </summary>
    public class sys_InteractionLearnsManager : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "getPageList")
            {
                GetPageList(context);
            }
            else
            {
                context.Response.Write("错误的请求");
            }
        }

        private void GetPageList(HttpContext context)
        {
            var size = context.Request["size"].ToInt();
            var page = context.Request["page"].ToInt();
            var type = context.Request["pageType"].ToInt();
            var title = (context.Request["title"] ?? "").Replace("  ", " ").Split(' ');
            var sh = new SqlHelper<sys_InteractionLearnsModel>(new sys_InteractionLearnsModel())
            {
                PageConfig = new PageConfig
                {
                    PageSize = size,
                    SortEnum = SortEnum.Desc,
                    PageSortField = "LearnId",
                    PageIndex = page
                }
            };
            if (type > 0)
            {
                sh.AddWhere("LearnType", type);
            }
            for (var i = 0; i < title.Length; i++)
            {
                if (title[i].IsNullOrEmpty()) continue;
                if (i == 0)
                    sh.AddWhere(" AND ( LearnTitle LIKE '%" + title[i] + "%' ");
                else if (i == title.Length - 1)
                    sh.AddWhere(" OR LearnTitle LIKE '%" + title[i] + "%' ) ");
                else
                    sh.AddWhere(" OR LearnTitle LIKE '%" + title[i] + "%'  ");
            }
            var list = sh.Select();
            context.Response.Write(new
            {
                list = list.Select(x => new { }),
                page,
                totle = sh.Total,
                sql = sh.SqlString.ToString()
            }.ToJson());
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