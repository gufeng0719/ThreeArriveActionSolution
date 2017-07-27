using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using System.Web.SessionState;
using ThreeArriveAction.BLL;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_IntegralInfoManager 的摘要说明
    /// </summary>
    public class sys_IntegralInfoManager :ManagePage, IHttpHandler,IRequiresSessionState
    {
        private readonly sys_IntegralInfoBLL iiBLL = new sys_IntegralInfoBLL();
        public override void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "integralInfoList")
            {
                IntegralInfoList(context);
            }else if (type == "get")
            {
                GetIntergralInfoList(context);
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

        private void GetIntergralInfoList(HttpContext context)
        {
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryIntValue("page");
            int iid = MXRequest.GetQueryIntValue("iid");
            StringBuilder strJson = new StringBuilder();
            string strWhere = " IntegralId=" +iid;
            string filedOrder = "IntegralDate DESC ";
            int recordCount = 0;
            DataSet ds = iiBLL.GetList(pageSize,pageIndex,strWhere,filedOrder,out recordCount);
            strJson.Append("{\"total\":" + recordCount);
            if (recordCount > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            string pageContent = Utils.OutPageList(pageSize, pageIndex, recordCount, "Load(__id__)", 8);
            if (pageContent == "")
            {
                strJson.Append(",\"pageContent\":\"\"");
            }
            else
            {
                strJson.Append(",\"pageContent\":" + pageContent);
            }
            strJson.Append("}");
            context.Response.Output.Write(strJson.ToString());
        }

        public new  bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}