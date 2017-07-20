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
                case "getsubfamily":
                    GetSubfamily(context);
                    break;

            }
        }

        private void GetSubcriberFamilyByUserId(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href=http://'" + HttpContext.Current.Request.Url.Authority + "/login.html'</script>");
                return;
            }
            else
            {
                string result = subBLL.GetSubscriberFamilyStringByUserId(model.UserId);
                context.Response.Write(result);
            }
        }


        public void GetSubfamily(HttpContext context)
        {
            var value = context.Request["value"];
            var openId = context.Request["openId"];
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere("UserRemark", openId);
            var user = sh.Select().FirstOrDefault();
            if (user == null)
            {
                LogHelper.Log("sys_SubscriberFamilyManager-->GetSubfamily-->opneId(" + openId + ")未能查询到用户信息");
                return;
            }
            var sh1 = new SqlHelper<sys_SubscriberFamilyModel>(new sys_SubscriberFamilyModel());
            sh1.AddWhere("SubscriberType", value);
            sh1.AddWhere("UserId", user.UserId);
            context.Response.Write(
                sh1.Select()
                    .Select(x => new
                    {
                        key = x.SubscriberId,
                        value = x.SubscriberName + "---" + x.SubscriberPhone
                    }).ToJson());
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