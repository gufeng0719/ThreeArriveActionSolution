
using System;
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
            else if (type == "getToUrl")
            {
                context.Response.Write(GetToUrl(context.Request["page"].ToInt()));
                context.Response.End();
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
            var response = GetAccessToken(context.Request["code"]);
            var toUrl = GetToUrl(context.Request["page"].ToInt());
            context.Response.Write(new
            {
                response.openid,
                toUrl
            }.ToJson());
            context.Response.End();
        }

        public void GetJsapiTicket(HttpContext context)
        {
            var aModel = CacheHelper.Get<AccessToken>("AccessToken");
            if (aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) < DateTime.Now)
            {

            }
            else
            {
                aModel = new AccessToken
                {
                    Time = DateTime.Now,
                    Value = GetAccessToken(context.Request["code"]).access_token
                };
            }
        }

        private string GetToUrl(int page)
        {
            var toUrl = ConfigurationManager.AppSettings["Localhost"] + "/weixin/";
            switch (page)
            {
                case 0:
                    toUrl += "sign.html";
                    break;
                // todo 根据 page 区分需要跳转的url
                default:
                    toUrl += "sign.html";
                    break;
            }
            return toUrl;
        }

        private OpenIdModel GetAccessToken(string code)
        {
            string url = string.Format(ConfigurationManager.AppSettings["WinxinOpenIdUrl"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"],
                                        code);

            var response = new HttpHelper().HttpGet(url).JsonToObject<OpenIdModel>();
            LogHelper.Log($"获取access_token:{response.access_token} \r\n    openid:{response.openid}", "记录每次调用access_token接口的时间", LogTypeEnum.Info);
            return response;
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

        class AccessToken
        {
            public string Value;
            public DateTime Time;
        }
    }
}