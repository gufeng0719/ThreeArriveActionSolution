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
            var openId = context.Request["openId"];
            if (model == null)
            {
                if (openId.IsNullOrEmpty())
                {
                    context.Response.Write("<script>parent.location.href='login.html'</script>");
                    return;
                }
                model = GetUserIdByOpenId(openId);
            }
            var wmodel =
                new sys_WeekArrivesModel
                {
                    SubcriberId = context.Request.Form["slSubId"].ToInt(),
                    WeekArriveDate = DateTime.Now,
                    ThingMessage = context.Request.Form["ThingMessage"],
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}