using System;
using System.Collections;
using System.Web;
using System.Data;
using System.Text;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using System.Web.SessionState;
using System.Linq;
using System.Collections.Generic;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_SignManager 的摘要说明
    /// </summary>
    public class sys_SignManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_SignsBLL signsBLL = new sys_SignsBLL();
        private readonly sys_OnSignsBLL onSignsBLL = new sys_OnSignsBLL();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string type = context.Request.QueryString["type"].ToString();
            switch (type)
            {
                case "get":
                    GetSignInfo(context);
                    break;
                case "add":
                    AddSignInfo(context);
                    break;
                case "getSignList":
                    GetSignList(context);
                    break;
                case "search":
                    SearchUserSign(context);
                    break;
                case "statis":
                    StatisticSign(context);
                    break;
            }
        }

        #region 获取该人签到信息
        private void GetSignInfo(HttpContext context)
        {
            var openId = context.Request["openId"];
            JsonMessage json = new JsonMessage();
            //获取本人信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                if (openId.IsNullOrEmpty())
                {
                    context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");
                    return;
                }
                else
                {
                    var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
                    sh.AddWhere("UserRemark", openId);
                    model = sh.Select().FirstOrDefault();
                    if (model == null)
                    {
                        context.Response.Write(new
                        {
                            success = false,
                            msg = "个人信息异常"
                        }.ToJson());
                        return;
                    }
                }
            }

            //根据本人的编号与村居编号获取本人签到信息
            if (onSignsBLL.ExistsByVillageId(model.VillageId))//是否开启签到
            {
                //签到已开启
                //查询本人是否开启签到
                if (signsBLL.Exists(model.UserId, DateTime.Now))
                {
                    //本人已签到
                    json.success = false;
                    json.msg = "您今天已签到,不需再签到!";
                }
                else
                {
                    DateTime onTime = onSignsBLL.GetOnSignsByVillageId(model.VillageId, DateTime.Now).OnTime;
                    if (onTime.AddMinutes(5) <= DateTime.Now)
                    {
                        json.success = false;
                        json.msg = "今天签到已结束,明天请提前!";

                    }
                    else
                    {
                        json.success = true;
                        json.msg = "您今天还未签到,现在签到吧!";
                        json.obj = onTime;
                    }
                }
            }
            else
            {
                //签到未开启
                if (model.OrganizationId != 3)
                {
                    json.success = false;
                    json.msg = "签到还没开启,请稍后!";

                }
                else
                {
                    json.success = true;
                    json.msg = "签到还没开启,开启签到吧!";
                }
            }
            context.Response.Write(JsonHelper.ToJson(json));
        }
        #endregion

        #region 添加签到信息
        public void AddSignInfo(HttpContext context)
        {//获取本人信息
            var openId = context.Request["openId"];
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                if (openId.IsNullOrEmpty())
                {
                    context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");

                    return;
                }
                else
                {
                    var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
                    sh.AddWhere("UserRemark", openId);
                    model = sh.Select().FirstOrDefault();
                    if (model == null)
                    {
                        context.Response.Write(new
                        {
                            success = false,
                            msg = "个人信息异常"
                        }.ToJson());
                        return;
                    }
                }
            }

            string result = signsBLL.AddSign(model.UserId, model.VillageId, model.OrganizationId);
            context.Response.Write(result);
        }
        #endregion

        #region 获取已经签到的列表
        public void GetSignList(HttpContext context)
        {
            var villageId = context.Request["villageId"].ToInt();
            var userSh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            userSh.AddWhere("VillageId", villageId);
            var needSign = userSh.Select();
            var sysUsersModels = needSign as sys_UsersModel[] ?? needSign.ToArray();
            if (!sysUsersModels.Any())
            {
                context.Response.Write(new ArrayList().ToJson());
                return;
            }
            var signSh = new SqlHelper<sys_SignsModel>(new sys_SignsModel());
            signSh.AddWhere(" AND SignDate >= '" + DateTime.Now.ToString("yyyy-MM-dd") +
                            "' AND SignUserId IN (" + string.Join(",", sysUsersModels.Select(x => x.UserId)) + ")");
            signSh.AddOrder("SignDate", SortEnum.Asc);
            var sign = signSh.Select();
            var sysSignsModels = sign as sys_SignsModel[] ?? sign.ToArray();

            context.Response.Write(sysSignsModels.ToList().Select(x => new
            {
                time = x.SignDate.ToString("yyyy-M-d hh:mm:ss"),
                name = sysUsersModels.FirstOrDefault(s => s.UserId == x.SignUserId)?.UserName ?? "管理员"
            }).ToJson());
        }
        #endregion

        #region 查询每个村每日早报道情况
        private void SearchUserSign(HttpContext context)
        {
            int pageSize = 20;
            int pageIndex = MXRequest.GetQueryInt("page");
            string signdate = MXRequest.GetQueryString("signdate");
            int town = MXRequest.GetQueryInt("town");
            int vid = MXRequest.GetQueryInt("vid");
            string strWhere2 = "";
            if (signdate.Trim() != "")
            {
                strWhere2 = " Convert(varchar(100),SignDate,23)='" + signdate + "'";
            }
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
            string fieldOrder = " UserId ASC ";
            int recordCount = 0;
            DataSet ds = signsBLL.SearchUserSign(pageSize, pageIndex, strWhere1, strWhere2, fieldOrder, out recordCount);
            List<sys_VillagesModel> villageList = DataConfig.GetVillages();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                dr["VillageName"] = villageList.FirstOrDefault(x => x.VillageId == int.Parse(dr["VillageParId"].ToString())).VillageName + "--" + dr["VillageName"].ToString();
            }
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{\"total\":" + recordCount);
            if (ds.Tables[0].Rows.Count > 0)
            {
                strJson.Append(",\"rows\":" + JsonHelper.ToJson(ds.Tables[0]));
            }
            else
            {
                strJson.Append(",\"rows\":[]");
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
            context.Response.Write(strJson.ToString());

        }
        #endregion

        #region 统计早报道
        private void StatisticSign(HttpContext context)
        {
            int rtype = MXRequest.GetQueryInt("rtype");
            StringBuilder strJson = new StringBuilder();
            int town = MXRequest.GetQueryInt("town");
            int vid = MXRequest.GetQueryInt("vid");
            StringBuilder strSql1 = new StringBuilder();
            StringBuilder strSql2 = new StringBuilder();
            if (town != 0 && vid == 0)
            {
                //每个镇的每个村委单位
                strSql1.Append("select count(1) as TotalNumber,VillageName as Name,a.VillageId as Id,0 HavNumber from");
                strSql1.Append(" sys_Users a inner join sys_Villages b on a.VillageId=b.VillageId");
                strSql1.Append(" where b.VillageParId=" + town);
                strSql1.Append(" group by VillageName,a.VillageId ");
                strSql2.Append("select count(1) as SignNumber from sys_Signs a inner join sys_Users b on a.SignUserId=b.UserId ");
                if (rtype == 0)
                {
                    //按周统计
                    string swdate = MXRequest.GetQueryString("swdate");
                    strSql2.Append(" where  datepart(wk,SignDate) = datepart(wk,'" + swdate + "') and VillageId =");
                }
                else
                {
                    //按月统计
                    string smdate = MXRequest.GetQueryString("smdate");
                    strSql2.Append(" where  Convert(varchar(7),SignDate,23) ='" + smdate + "' and VillageId=");
                }

            }
            else if (town != 0 && vid != 0)
            {
                //村每个工作人员为单位
                strSql1.Append("select count(1) as TotalNumber,UserName as Name,UserId as Id,0 HavNumber from sys_Users ");
                strSql1.Append(" where VillageId=" + vid);
                strSql1.Append("Group by UserName,UserId  ");
                strSql2.Append("select count(1) as SignNumber from sys_Signs a inner join sys_Users b on a.SignUserId=b.UserId ");
                if (rtype == 0)
                {
                    //按周统计
                    string swdate = MXRequest.GetQueryString("swdate");
                    strSql2.Append(" where  datepart(wk,SignDate) = datepart(wk,'" + swdate + "') and SignUserId =");
                }
                else
                {
                    //按月统计
                    string smdate = MXRequest.GetQueryString("smdate");
                    strSql2.Append(" where  Convert(varchar(7),SignDate,23) ='" + smdate + "' and SignUserId=");
                }
            }
            
            DataTable dt = signsBLL.StatisticsSign(strSql1.ToString(), strSql2.ToString());
            strJson.Append("{\"total\":"+dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                strJson.Append(",\"rows\":"+JsonHelper.ToJson(dt));
            }
            else
            {
                strJson.Append(",\"rows\":[]");
            }
            if (rtype == 0)
            {
                strJson.Append(",\"workdays\":5");
            }
            else
            {
                strJson.Append(",\"workdays\":"+ GetWorkDay(MXRequest.GetQueryString("smdate")));
            }
            strJson.Append("}");
            context.Response.Output.Write(strJson.ToString());
        }
        #endregion

        private int GetWorkDay(string searchTimes)
        {
            DateTime dt = Convert.ToDateTime(searchTimes + "-01");    // 当前日期月份的第一天
            int year =dt.Year; // 获得年
            int month = dt.Month;// 获得月
            int days = DateTime.DaysInMonth(year, month);     // 获得该月总共多少天

            // 休息天数
            int weekDays = 0;

            for (int i = 0; i < days; i++)
            {
                // 判断是否为周六，周日，是则记录天数。
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Saturday:
                        weekDays++;
                        break;
                    case DayOfWeek.Sunday:
                        weekDays++;
                        break;
                }
                dt = dt.AddDays(1);
            }
            // 工作日
            int workDays = days - weekDays;
            return workDays;
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