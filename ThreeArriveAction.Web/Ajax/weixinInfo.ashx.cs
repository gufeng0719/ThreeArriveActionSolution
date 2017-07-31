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
using ThreeArriveAction.Model;

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
            else if (type == "sendMsgTest")
            {
                SendMsgTest(context);
            }
            else
            {
                context.Response.Write("错误的请求");
                context.Response.End();
            }
        }

        public void SendMsgTest(HttpContext context)
        {
            // 参数传入以及处理
            var openIds = (context.Request["openIds[]"] ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var msg = context.Request["msg"];
            var type = context.Request["msgType"].ToInt();
            var path = context.Request["path"];
            var title = context.Request["title"];
            var describe = context.Request["describe"];
            var twList = context.Request["twList"].Replace(@"\\", "/").JsonToObject<List<TwModel>>();
            var arr = new ArrayList();
            arr.AddRange(openIds);
            if (arr.Count < 2)
            {
                arr.Add(ConfigurationManager.AppSettings["OpenIdForAdmin"]);
            }
            // 获取 access token 
            var aModel = CacheHelper.Get<Token>("_AccessToken");
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now))
            {
                aModel = GetAModel();
            }

            // 如果有文件则先获取文件的madiaId
            var media = new MediaModel();
            if (!path.IsNullOrEmpty())
            {
                media = HttpHelper.UploadMultimedia(path, aModel.Value, MediaType.image).JsonToObject<MediaModel>();
            }

            var body = new object(); // 请求参数
            // 针对不同类型发送信息
            if (type == MediaType.text.GetHashCode()) // 发送文本信息
            {
                body = new
                {
                    touser = arr[0],
                    msgtype = "text",
                    text = new
                    {
                        content = msg
                    }
                };
            }
            else if (type == MediaType.voice.GetHashCode() || type == MediaType.image.GetHashCode()) // 图片和语音
            {
                body = new
                {
                    touser = arr[0],
                    image = new
                    {
                        media.media_id
                    },
                    msgtype = "image"
                };
            }
            else if (type == MediaType.video.GetHashCode()) // 视频消息
            {
                var videoMediaStr = HttpHelper.HttpPost("https://api.weixin.qq.com/cgi-bin/media/uploadvideo?access_token=" + aModel.Value, new
                {
                    title,
                    media.media_id,
                    description = describe
                }.ToJson());
                LogHelper.Log(videoMediaStr, media.media_id);
                body = new
                {
                    touser = arr[0],
                    mpvideo = new
                    {
                        title,
                        videoMediaStr.JsonToObject<MediaModel>().media_id,
                        description = describe
                    },
                    msgtype = "mpvideo"
                };
            }
            else if (type == MediaType.news.GetHashCode()) // 图文消息
            {
                var articles = new ArrayList();
                foreach (var tw in twList)
                {
                    var twMedia = HttpHelper.UploadMultimedia(tw.path, aModel.Value, MediaType.image).JsonToObject<MediaModel>();
                    var sh = new SqlHelper<sys_MajorInfoModel>(new sys_MajorInfoModel
                    {
                        MajorFromUserId = 1,
                        MajorContent = tw.msg,
                        MajorTitle = tw.title,
                        MajorDate = DateTime.Now
                    });
                    var ident = sh.Insert();
                    articles.Add(new
                    {
                        thumb_media_id = twMedia.media_id ?? "",
                        content_source_url = ConfigurationManager.AppSettings["Localhost"] + "/weixin/majorInfo.html?id=" + ident,
                        author = "管理员",
                        tw.title,
                        content = tw.msg + "<br>=================================<br>请点击右下角 的阅读原文, 且提交您的阅读结果",
                        show_cover_pic = 1
                    });
                    // 插入重要信息

                }
                var twBody = new
                {
                    articles
                };
                var twRes = HttpHelper.HttpPost("https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token=" + aModel.Value,
                                                twBody.ToJson());
                body = new
                {
                    touser = arr[0],
                    mpnews = new
                    {
                        twRes.JsonToObject<MediaModel>().media_id
                    },
                    msgtype = "mpnews"
                };
            }
            var res = HttpHelper.HttpPost(ConfigurationManager.AppSettings["WeixinSendMsgTest"] + aModel.Value,
                                          body.ToJson());
            LogHelper.Log(body.ToJson(), ConfigurationManager.AppSettings["WeixinSendMsgTest"] + aModel.Value);
            context.Response.Write(res);
        }

        public void SendMsg(HttpContext context)
        {
            var openIds = context.Request["openIds[]"].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var msg = context.Request["msg"];
            var type = context.Request["msgType"].ToInt();
            var path = context.Request["path"];
            var title = context.Request["title"];
            var arr = new ArrayList();
            arr.AddRange(openIds);
            if (arr.Count < 2)
            {
                arr.Add(ConfigurationManager.AppSettings["OpenIdForAdmin"]);
            }
            var aModel = CacheHelper.Get<Token>("_AccessToken");
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now))
            {
                aModel = GetAModel();
            }
            object body;
            if (type == MediaType.text.GetHashCode()) // 发送文本信息
            {
                body = new
                {
                    touser = arr,
                    msgtype = "text",
                    text = new
                    {
                        content = msg
                    }
                };
            }
            else if (type == MediaType.voice.GetHashCode() || type == MediaType.image.GetHashCode())// 图片和语音
            {
                var media = HttpHelper.UploadMultimedia(path, aModel.Value, MediaType.image).JsonToObject<MediaModel>();
                body = new
                {
                    touser = openIds,
                    image = new
                    {
                        media.media_id
                    },
                    msgtype = "image"
                };
            }
            else
            {
                var media = HttpHelper.UploadMultimedia(path, aModel.Value, MediaType.video).JsonToObject<MediaModel>();
                var videoMedia = HttpHelper.HttpPost("https://api.weixin.qq.com/cgi-bin/media/uploadvideo?access_token=" + aModel.Value, new
                {
                    title,
                    media.media_id,
                    description = msg
                }.ToJson()).JsonToObject<MediaModel>();
                body = new
                {
                    touser = openIds,
                    mpvideo = new
                    {
                        title,
                        videoMedia.media_id,
                        description = msg
                    },
                    msgtype = "mpvideo"
                };
            }
            var res = HttpHelper.HttpPost(ConfigurationManager.AppSettings["WeixinSendMsg"] + aModel.Value, body.ToJson());
            LogForSendMsg(aModel.Value, body, res);
            //if (res.IndexOf("success", StringComparison.Ordinal) < 0)
            //{
            // todo 只有失败记录日志,目前测试在任何事件都记录了日志
            //}
            context.Response.Write(res);
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
            return new JdkConfig
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

            var response = HttpHelper.HttpGet(url).JsonToObject<OpenIdModel>();
            LogHelper.Log("获取access_token:" + response.access_token + " \r\n    openid:" + response.openid, "记录每次调用access_token接口的时间");
            return response;
        }

        private Token GetAModel()
        {
            var url = string.Format(ConfigurationManager.AppSettings["WeixinAccessToken"],
                                        ConfigurationManager.AppSettings["AppId"],
                                        ConfigurationManager.AppSettings["AppSecret"]);
            var accessToken = HttpHelper.HttpGet(url).JsonToObject<AccessToken>();
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
            var jticket = HttpHelper.HttpGet(url).JsonToObject<JsapiTicket>();
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

        private void LogForSendMsg(string accessToken, object body, string res)
        {
            LogHelper.Log(ConfigurationManager.AppSettings["WeixinSendMsg"] + accessToken, "群发消息请求链接");
            LogHelper.Log(body.ToJson(), "群发消息Body");
            LogHelper.Log(res, "群发消息响应");
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

        class JdkConfig
        {
            public string appId = ConfigurationManager.AppSettings["AppId"];
            public string timestamp;
            public string nonceStr;
            public string signature;
            public List<string> jsApiList;
        }

        class MediaModel
        {
            public string type;
            public string media_id;
            public string created_at;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class TwModel
        {
            public string title;
            public string msg;
            public string path;
        }
    }
}