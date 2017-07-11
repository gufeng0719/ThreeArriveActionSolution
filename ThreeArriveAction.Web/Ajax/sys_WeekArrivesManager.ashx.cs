using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            }


        }

        public void AddWeekArrives(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            var openId = context.Request.Form["openId"];
            if (model == null && openId.IsNullOrEmpty())
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                sys_WeekArrivesModel wmodel = new sys_WeekArrivesModel();
                wmodel.SubcriberId = int.Parse(context.Request.Form["slSubId"].ToString());
                wmodel.WeekArriveDate = DateTime.Now;
                wmodel.ThingMessage = context.Request.Form["ThingMessage"].ToString();
                wmodel.ThingResult = context.Request.Form["ThingResult"].ToString();
                wmodel.ThingImgUrl = context.Request.Form["txtImgUrl"].ToString();
                wmodel.UserId = model == null ? GetUserIdByOpenId(openId) : model.UserId;
                wmodel.WeekArriveState = 1;

                string xpoint = context.Request.Form["xpoint"].ToString();
                string ypoint = context.Request.Form["ypoint"].ToString();
                wmodel.Remarks = xpoint + "," + ypoint;
                string result = weekBLL.AddWeekArrives(wmodel);
                context.Response.Write(result);
            }
        }

        public int GetUserIdByOpenId(string openId)
        {
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", openId, RelationEnum.Like);
            var model = sh.Select().FirstOrDefault();
            if (model == null)
            {
                LogHelper.Log("根据OpenId(" + openId + ")未能查询到人员信息");
                return 0;
            }
            else
            {
                return model.UserId;
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