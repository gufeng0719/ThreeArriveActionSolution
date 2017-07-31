using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.BLL;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_WeekArrivesManager 的摘要说明
    /// </summary>
    public class sys_WeekArrivesManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_WeekArrivesBLL weekBLL = new sys_WeekArrivesBLL();
        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //取得处事类型
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "add":
                    AddWeekArrives(context);
                    break;
                case "search":
                    SearchUserWeekArrive(context);
                    break;
            }


        }

        public void AddWeekArrives(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            var openId = context.Request["openId"];
            if (model == null)
            {
                if (openId.IsNullOrEmpty())
                {
                    context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");
                    return;
                }
                model = GetUserIdByOpenId(openId);
            }
            var wmodel =
                new sys_WeekArrivesModel
                {
                    SubcriberId = context.Request.Form["slSubId"].ToInt(),
                    WeekArriveDate = DateTime.Now,
                    ThingMessage = context.Request.Form["ThingMessage"] ?? "",
                    ThingResult = context.Request.Form["ThingResult"],
                    ThingImgUrl = context.Request.Form["txtImgUrl"],
                    UserId = model.UserId,
                    WeekArriveState = 1
                };

            string xpoint = context.Request.Form["xpoint"];
            string ypoint = context.Request.Form["ypoint"];
            wmodel.Remarks = xpoint + "," + ypoint;
            string result = weekBLL.AddWeekArrives(wmodel);
            context.Response.Write(result);
        }

        public sys_UsersModel GetUserIdByOpenId(string openId)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", openId, RelationEnum.Like);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                LogHelper.Log("根据OpenId(" + openId + ")未能查询到人员信息");
                return null;
            }
            return model;
        }

        private void SearchUserWeekArrive(HttpContext context)
        {
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryInt("page");
            string searchdate = MXRequest.GetQueryString("weekdate");
            int town = MXRequest.GetQueryInt("weektown");
            int vid = MXRequest.GetQueryInt("weekvid");
            string strWhere1 = "";
            if (town != 0 && vid == 0)
            {
                strWhere1 = "sys_Villages.VillageId=" + town + " or sys_Villages.VillageParId=" + town;
            }
            else if (town != 0 && vid != 0)
            {
                strWhere1 = "sys_Villages.VillageId=" + vid;
            }
            else if (town == 0 && vid != 0)
            {
                strWhere1 = "sys_Villages.VillageId=" + vid;
            }
            else
            {
                strWhere1 = "sys_Villages.VillageId!=1";
            }
            string strWhere2 = "";
            if (searchdate.Trim() != "")
            {
                strWhere2 = " datepart(wk,WeekArriveDate) =datepart(wk,'"+searchdate+"') ";
            }else
            {
                strWhere2 = " datepart(wk,WeekArriveDate) =datepart(wk,getdate())";
            }
            string fieldOrder = " SubScriberId DESC ";
            int recordCount = 0;
            DataSet ds = weekBLL.SearchUserWeekArrive(pageSize, pageIndex, strWhere1, strWhere2, fieldOrder, out recordCount);
            List<sys_VillagesModel> villageList = DataConfig.GetVillages();
            foreach(DataRow dr in ds.Tables[0].Rows)
            {
                dr["VillageName"] = villageList.FirstOrDefault(x => x.VillageId == int.Parse(dr["VillageParId"].ToString())).VillageName + "--" + dr["VillageName"].ToString();
            }
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{\"total\":"+ recordCount);
            if (ds.Tables[0].Rows.Count > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            else
            {
                strJson.Append(",\"rows\":[]");
            }
            string pageContent = Utils.OutPageList(pageSize, pageIndex, recordCount, "LoadWeek(__id__)", 8);
            if (pageContent == "")
            {
                strJson.Append(",\"pageContent\":\"\"");
            }
            else
            {
                strJson.Append(",\"pageContent\":" + pageContent);
            }

            strJson.Append("}");
            context.Response.Write(strJson.ToString());
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