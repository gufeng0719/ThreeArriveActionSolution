using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.Common;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.Model;
namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_SubscriberFamilyManager 的摘要说明
    /// </summary>
    public class sys_SubscriberFamilyManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_SubscriberFamilyBLL subBLL = new sys_SubscriberFamilyBLL();
        public void ProcessRequest(HttpContext context)
        {
            //取得处事类型
            string type = MXRequest.GetQueryString("type");

            switch (type)
            {
                case "get": //获取对应七户信息
                    GetSubcriberFamilyByUserId(context);
                    break;

            }
        }

        private void GetSubcriberFamilyByUserId(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                string result = subBLL.GetSubscriberFamilyStringByUserId(model.UserId);
                context.Response.Write(result);
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