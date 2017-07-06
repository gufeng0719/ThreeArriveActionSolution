
using System.Configuration;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.Common;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// weixinInfo 的摘要说明
    /// </summary>
    public class weixinInfo : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var type = context.Request["type"];
            if (type == "getCodeUrl")
            {
                GetCodeUrl(context);
            }
            else if (type == "getOpenInfo")
            {
                GetUserInfo(context);
            }
            else
            {
                context.Response.Write("错误的请求");
                context.Response.End();
            }
        }


        public void GetCodeUrl(HttpContext context)
        {
            var url = string.Format(ConfigurationManager.AppSettings["WinxinCodeUrl"], 
                                    ConfigurationManager.AppSettings["AppId"],
                                    ConfigurationManager.AppSettings["Localhost"]
                                        + "/weixin/redirect_uri.html"
                                        + "?page=" + context.Request["page"]);
            context.Response.Write(url);
            context.Response.End();
        }

        public void GetUserInfo(HttpContext context)
        {
            string url = string.Format(ConfigurationManager.AppSettings["WinxinOpenIdUrl"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"],
                                        context.Request["code"]);

            var response = new HttpHelper().HttpGet(url).JsonToObject<OpenIdModel>();

            var toUrl = ConfigurationManager.AppSettings["Localhost"] + "/weixin/"; 
            switch (context.Request["page"])
            {
                case "0":
                    toUrl += "sign.html";
                    break;
                // todo 根据 page 区分需要跳转的url
                default:
                    toUrl += "sign.html";
                    break;
            }
            context.Response.Write(new
            {
                response.openid,
                toUrl
            }.ToJson());
            context.Response.End();
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        class OpenIdModel
        {
            public string access_token;
            public string expires_in;
            public string refresh_token;
            public string openid;
            public string scope;
        }
    }
}