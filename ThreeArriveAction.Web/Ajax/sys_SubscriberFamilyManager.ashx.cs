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
                case "getSubfamilyModel":
                    GetSubfamilyModel(context);
                    break;
                case "addSubfamily":
                    AddSubfamily(context);
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

        public void GetSubfamily(HttpContext context)
        {
            var value = context.Request["value"];
            var openId = context.Request["openId"];
            var sh = new SqlHelper<sys_UsersModel>(new sys_UsersModel());
            sh.AddWhere(" AND UserRemark='" + openId + "'");
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

        public void GetSubfamilyModel(HttpContext context)
        {
            var id = context.Request["id"].ToInt();
            var sh = new SqlHelper<sys_SubscriberFamilyModel>(new sys_SubscriberFamilyModel());
            sh.AddWhere("SubscriberId", id);
            var model = sh.Select().FirstOrDefault();
            if (model != null)
            {
                var arr = model.FamilyCoordinate.Split(',');
                var x = 33.5226;
                var y = 119.156456;
                if (arr.Length == 2)
                {
                    x = arr[0].ToDouble();
                    y = arr[1].ToDouble();
                }
                context.Response.Write(new
                {
                    name = model.SubscriberName,
                    phone = model.SubscriberPhone ?? "",
                    address = model.FamilyAddress ?? "",
                    msg = model.Remarks ?? "",
                    number = model.FamilyNumber,
                    x,
                    y
                }.ToJson());
                return;
            }
            context.Response.Write("未能查询到七户信息 Id:" + id);
        }

        public void AddSubfamily(HttpContext context)
        {
            var id = context.Request["family"].ToInt();
            var name = context.Request["name"];
            var phone = context.Request["phone"];
            var address = context.Request["address"];
            var msg = context.Request["msg"];
            var number = context.Request["number"].ToInt();
            var x = context.Request["x"].ToDouble();
            var y = context.Request["y"].ToDouble();
            var sh = new SqlHelper<sys_SubscriberFamilyModel>(new sys_SubscriberFamilyModel());
            sh.AddWhere("SubscriberId", id);
            sh.AddUpdate("SubscriberName", name);
            sh.AddUpdate("SubscriberPhone", phone);
            sh.AddUpdate("FamilyAddress", address);
            sh.AddUpdate("Remarks", msg);
            sh.AddUpdate("FamilyNumber", number);
            sh.AddUpdate("FamilyCoordinate", x + "," + y);
            var line = sh.Update();
            context.Response.Write(new
                                   {
                line
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