using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using System.Web.SessionState;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.BLL;


namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_UserIntegralsManager 的摘要说明
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class sys_UserIntegralsManager :ManagePage, IHttpHandler,IRequiresSessionState
    {
        private readonly sys_UserIntergralsBLL uIBLL = new sys_UserIntergralsBLL();
        public override void ProcessRequest(HttpContext context)
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
            else if (type == "get")
            {
                GetUserIntergalList(context);
            }else if (type == "info")
            {
                //GetIntegralInfo(context);
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

        private void GetUserIntergalList(HttpContext context)
        {
            int town = MXRequest.GetQueryIntValue("town");
            int vid = MXRequest.GetQueryIntValue("vid");
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryIntValue("page");
            string sdate =context.Request.QueryString["sdate"].ToString();
            sys_UsersModel userModel = GetUsersinfo();
            StringBuilder strWhere = new StringBuilder();
            if (sdate.Trim() == "")
            {
                strWhere.Append(" IntegralYear="+DateTime.Now.Year+" and IntegralMonth="+DateTime.Now.Month);
            }
            else
            {
                strWhere.Append(" IntegralYear="+sdate.Split('-')[0]+" and IntegralMonth="+sdate.Split('-')[1]);
            }
            if (town == 0 && vid == 0)
            {//如果镇和村编号全部为空，根据用户角色查询
                if (userModel.OrganizationId == 1)
                {//超级管理员 查询全部
                    strWhere.Append(" and  1=1");
                }else if (userModel.OrganizationId == 2)
                {
                    //镇管理员，查询本镇
                    strWhere.Append(" and b.VillageId in (select VillageId from sys_Village where VillageId="+userModel.VillageId+" or VillageParId ="+userModel.VillageId+")");
                }else
                {
                    //村管理员和村居干部，查询本村
                    strWhere.Append(" and b.VillageId ="+userModel.VillageId);
                }
              
            }else if (town != 0 && vid == 0)
            {//如果镇不为空，村为空，则查询该镇所有
                strWhere.Append(" and b.VillageId in (select VillageId from sys_Village where VillageId=" + town + " or VillageParId =" + town + ")");
            }
            else
            {
                //如果镇为空，村不为空，或者镇不为空村也不为空，都查询该村
                strWhere.Append(" and b.VillageId =" + vid);
            }
            string filedOrder = " IntegralScore DESC ";
            int recordCount = 0;
            var villageList = DataConfig.GetVillages();
            DataSet ds = uIBLL.GetList(pageSize, pageIndex, strWhere.ToString(), filedOrder, out recordCount);
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                dr["VillageName"] = villageList.FirstOrDefault(x => x.VillageId == int.Parse(dr["VillageParId"].ToString())).VillageName + "--" + dr["VillageName"].ToString();
            }
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{");
            strJson.Append("\"total\":" + recordCount);
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

        #region 获取该积分详细
        
        #endregion

        public new bool IsReusable
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