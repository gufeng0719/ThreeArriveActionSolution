
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.Common;
using System.Drawing;

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
            else if (type == "getJsapiTicket")
            {
                GetJsapiTicket(context);
            }
            else if (type == "downFile")
            {
                FileDown(context);
            }
            else if (type == "sendMsg")
            {
                SendMsg(context);
            }
            else
            {
                context.Response.Write("错误的请求");
                context.Response.End();
            }
        }

        public void SendMsg(HttpContext context)
        {
            var openIds = context.Request["openIds[]"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var msg = context.Request["msg"];
            var arr = new ArrayList();
            arr.AddRange(openIds);
            arr.Add(ConfigurationManager.AppSettings["OpenIdForAdmin"]);
            var body = new
            {
                touser = arr,
                msgtype = "text",
                text = new { content = msg }
            };
            var aModel = CacheHelper.Get<Token>("_AccessToken");
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now))
            {
                aModel = GetAModel();
            }
            var res = new HttpHelper().HttpPost(ConfigurationManager.AppSettings["WeixinSendMsg"] + aModel.Value, body.ToJson());
            if (res.IndexOf("success", StringComparison.Ordinal) < 0)
            {
                LogHelper.Log(ConfigurationManager.AppSettings["WeixinSendMsg"] + aModel.Value, "群发消息请求链接");
                LogHelper.Log(body.ToJson(), "群发消息Body");
                LogHelper.Log(res, "群发消息响应");
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
            context.Response.Write(new
            {
                response.openid,
            }.ToJson());
            context.Response.End();
        }

        public void GetJsapiTicket(HttpContext context)
        {
            var openId = context.Request["openid"];
            var jModel = CacheHelper.Get<Token>("_JsapiTicket");
            if (!(jModel != null && !jModel.Value.IsNullOrEmpty() && jModel.Time.AddHours(2) >= DateTime.Now))
            {
                var aModel = CacheHelper.Get<Token>("_AccessToken");
                if (aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now)
                {
                    jModel = GetJModel(aModel.Value, openId);
                }
                else
                {
                    aModel = GetAModel();
                    jModel = GetJModel(aModel.Value, openId);
                }
            }
            context.Response.Write(GetJDKConfig(jModel.Value, context));
        }

        public void FileDown(HttpContext context)
        {
            var mediaId = context.Request["mediaId"];
            var openId = context.Request["openId"] ?? "";
            var aModel = CacheHelper.Get<Token>("_AccessToken");
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now))
            {
                aModel = GetAModel();
            }
            var path = SaveFile(aModel.Value, mediaId);
            context.Response.Write(path);
        }

        private string GetJDKConfig(string ticket, HttpContext context)
        {
            var nonceStr = ConvertHelper.GetNonce(16);
            var timestamp = ConvertHelper.GetTimeStamp();
            var str = "jsapi_ticket=" + ticket
                + "&noncestr=" + nonceStr
                + "&timestamp=" + timestamp
                + "&url=" + context.Request["url"];
            LogHelper.Log(str, "signature");
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

        private OpenIdModel GetAccessToken(string code)
        {
            string url = string.Format(ConfigurationManager.AppSettings["WeixinOpenIdUrl"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"],
                                        code);

            var response = new HttpHelper().HttpGet(url).JsonToObject<OpenIdModel>();
            LogHelper.Log("获取access_token:" + response.access_token + " \r\n    openid:" + response.openid, "记录每次调用access_token接口的时间");
            return response;
        }

        private Token GetAModel()
        {
            var url = string.Format(ConfigurationManager.AppSettings["WeixinAccessToken"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"]);
            var accessToken = new HttpHelper().HttpGet(url).JsonToObject<AccessToken>();
            LogHelper.Log("获取access_token:" + accessToken.access_token, "记录每次调用access_token接口的时间");
            var m = new Token
            {
                Time = DateTime.Now,
                Value = accessToken.access_token
            };
            CacheHelper.Insert("_AccessToken", m);
            return m;
        }

        private Token GetJModel(string token, string openId)
        {
            var url = string.Format(ConfigurationManager.AppSettings["WeixinJsapiTicket"],
                                        token);
            var jticket = new HttpHelper().HttpGet(url).JsonToObject<JsapiTicket>();
            LogHelper.Log("获取jsapi_ticket:" + jticket.ticket + " \r\n    openid:" + openId, "记录每次调用jsapi_ticket接口的时间");
            var m = new Token
            {
                Time = DateTime.Now,
                Value = jticket.ticket
            };
            CacheHelper.Insert("_JsapiTicket", m);
            return m;
        }

        private string SaveFile(string token, string mediaId)
        {
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                var url = string.Format(ConfigurationManager.AppSettings["WeixinFileDown"], token, mediaId);
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                Image image = Image.FromStream(webResponse.GetResponseStream());
                var repath = "/upload/" + DateTime.Now.ToString("yyyyMMdd") + "/" + DateTime.Now.ToString("yyyyMMddhhmmssffff") + "." + webResponse.ContentType.Split('/')[1];
                if (!Directory.Exists(path + "/upload/" + DateTime.Now.ToString("yyyyMMdd")))
                    Directory.CreateDirectory(path + "/upload/" + DateTime.Now.ToString("yyyyMMdd"));
                path += repath;
                image.Save(path);
                return repath;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex.Message + "-----path:" + path + ";mediaId:" + mediaId, "微信图片保存到服务器失败");
                return "";
            }

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