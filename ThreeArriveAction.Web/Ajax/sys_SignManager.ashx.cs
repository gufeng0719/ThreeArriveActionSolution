using System;
using System.Web;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using System.Web.SessionState;

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
            }
        }

        #region 获取该人签到信息
        private void GetSignInfo(HttpContext context)
        {
            JsonMessage json = new JsonMessage();
            //获取本人信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='../login.html'</script>");
            }
            else
            {
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
        }
        #endregion

        #region 添加签到信息
        public void AddSignInfo(HttpContext context)
        {//获取本人信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='../login.html'</script>");
            }
            else
            {
                string result = signsBLL.AddSign(model.UserId, model.VillageId, model.OrganizationId);
                context.Response.Write(result);
            }
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}