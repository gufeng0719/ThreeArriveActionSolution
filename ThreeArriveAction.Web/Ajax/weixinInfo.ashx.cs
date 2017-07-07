
using System;
using System.Collections.Generic;
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
            else if (type == "getJsapiTicket")
            {
                GetJsapiTicket(context);
            }
            else
            {
                context.Response.Write("错误的请求");
                context.Response.End();
            }
        }

        public void GetCodeUrl(HttpContext context)
        {
            var url = string.Format(ConfigurationManager.AppSettings["WeixinCodeUrl"],
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
            var url = "";
            var openId = context.Request["openid"];
            var jModel = CacheHelper.Get<Token>(openId + "_JsapiTicket");
            if (jModel != null && !jModel.Value.IsNullOrEmpty() && jModel.Time.AddHours(2) >= DateTime.Now)
            {
                context.Response.Write(GetJDKConfig(jModel.Value, context));
                return;
            }
            else
            {
                var aModel = CacheHelper.Get<Token>(openId + "_AccessToken");
                if (aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now)
                {
                    url = string.Format(ConfigurationManager.AppSettings["WeixinJsapiTicket"],
                                        aModel.Value);
                    var jticket = new HttpHelper().HttpGet(url).JsonToObject<JsapiTicket>();
                    LogHelper.Log("获取jsapi_ticket:" + jticket.ticket + " \r\n    openid:" + openId, "记录每次调用jsapi_ticket接口的时间", LogTypeEnum.Info);
                    CacheHelper.Insert(openId + "_JsapiTicket",
                        new Token
                        {
                            Time = DateTime.Now,
                            Value = jticket.ticket
                        });
                    context.Response.Write(GetJDKConfig(jticket.ticket, context));
                    return;
                }
                else
                {
                    url = string.Format(ConfigurationManager.AppSettings["WeixinAccessToken"],
                                       ConfigurationManager.AppSettings["AppId"],
                                       ConfigurationManager.AppSettings["AppSecret"]);
                    var accessToken = new HttpHelper().HttpGet(url).JsonToObject<AccessToken>();
                    LogHelper.Log("获取access_token:" + accessToken.access_token + " \r\n    openid:" + openId, "记录每次调用access_token接口的时间", LogTypeEnum.Info);
                    CacheHelper.Insert(openId + "_AccessToken",
                        new Token
                        {
                            Time = DateTime.Now,
                            Value = accessToken.access_token
                        });
                    url = string.Format(ConfigurationManager.AppSettings["WeixinJsapiTicket"],
                                        accessToken.access_token);
                    var jticket = new HttpHelper().HttpGet(url).JsonToObject<JsapiTicket>();
                    CacheHelper.Insert(openId + "_JsapiTicket",
                        new Token
                        {
                            Time = DateTime.Now,
                            Value = jticket.ticket
                        });
                    context.Response.Write(GetJDKConfig(jticket.ticket, context));
                    return;
                }
            }
        }

        private string GetJDKConfig(string ticket, HttpContext context)
        {
            var nonceStr = ConvertHelper.GetNonce(16);
            var timestamp = ConvertHelper.GetTimeStamp();
            var str = "jsapi_ticket=" + ticket
                + "&noncestr=" + nonceStr
                + "&timestamp=" + timestamp
                + "&url=" + context.Request.Url.ToString();
            return new JDKConfig
            {
                jsApiList = new List<string>
                {
                    "onMenuShareTimeline",
                    "onMenuShareAppMessage",
                    "onMenuShareQQ",
                    "onMenuShareWeibo",
                    "onMenuShareQZone",
                    "startRecord",
                    "stopRecord",
                    "onVoiceRecordEnd",
                    "playVoice",
                    "pauseVoice",
                    "stopVoice",
                    "onVoicePlayEnd",
                    "uploadVoice",
                    "downloadVoice",
                    "chooseImage",
                    "previewImage",
                    "uploadImage",
                    "downloadImage",
                    "translateVoice",
                    "getNetworkType",
                    "openLocation",
                    "getLocation",
                    "hideOptionMenu",
                    "showOptionMenu",
                    "hideMenuItems",
                    "showMenuItems",
                    "hideAllNonBaseMenuItem",
                    "showAllNonBaseMenuItem",
                    "closeWindow",
                    "scanQRCode",
                    "chooseWXPay",
                    "openProductSpecificView",
                    "addCard",
                    "chooseCard",
                    "openCard"
                },
                nonceStr = nonceStr,
                signature = DESEncrypt.Sha1(str),
                timestamp = timestamp
            }.ToJson();
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
            string url = string.Format(ConfigurationManager.AppSettings["WeixinOpenIdUrl"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"],
                                        code);

            var response = new HttpHelper().HttpGet(url).JsonToObject<OpenIdModel>();
            LogHelper.Log("获取access_token:" + response.access_token + " \r\n    openid:" + response.openid, "记录每次调用access_token接口的时间", LogTypeEnum.Info);
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

        class Token
        {
            public string Value;
            public DateTime Time;
        }

        class JsapiTicket
        {
            public string errcode;
            public string errmsg;
            public string ticket;
            public string expires_in;
        }

        class AccessToken
        {
            public string access_token;
            public string expires_in;
        }

        class JDKConfig
        {
            public string appId = ConfigurationManager.AppSettings["AppId"];
            public string timestamp;
            public string nonceStr;
            public string signature;
            public List<string> jsApiList;
        }
    }
}