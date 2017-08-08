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
                SendMsg(context, ConfigurationManager.AppSettings["WeixinSendMsg"], false);
            }
            else if (type == "sendMsgTest")
            {
                SendMsg(context, ConfigurationManager.AppSettings["WeixinSendMsgTest"], true);
            }
            else if (type == "sendTemplateMsg")
            {
                SendTemplateMsg(context);
            }
            else
            {
                context.Response.Write("错误的请求");
                context.Response.End();
            }
        }

        public void SendTemplateMsg(HttpContext context)
        {
            var aModel = CacheHelper.Get<Token>("_AccessToken");
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) < DateTime.Now))
            {
                aModel = GetAModel();
            }
            var fromName = context.Request["obj[fromName]"];
            var toName = context.Request["obj[toName]"];
            var toOpenId = context.Request["obj[toOpenId]"];
            var time = context.Request["obj[time]"];
            var title = context.Request["obj[title]"];
            var having = context.Request["obj[having]"];
            var res = HttpHelper.HttpPost("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=b4XSnZh4G2KW6CpHzQBsCDz0-G8KHnJFLVPMX54_vvHtP_CBhjNPqt774LmvGGaIAa0yYgh7xeUf34kWrM1OHpXjrBUCJUx_WkYJV6eJsMN1WRitRk96UjGqoBWNrL9ySDUhAGAEPG",
                                             new
                                             {
                                                 template_id = "H8N_H43s_RVHY9jtvEo7-9JKhr4rCUB8YrxFZ5ReQt8",
                                                 touser = toOpenId,
                                                 data = new
                                                 {
                                                     first = new
                                                     {
                                                         value = fromName.Substring(0, fromName.IndexOf("(", StringComparison.Ordinal)) + "拜访通知",
                                                     },
                                                     keyword1 = new
                                                     {
                                                         value = fromName,
                                                     },
                                                     keyword2 = new
                                                     {
                                                         value = toName,
                                                     },
                                                     keyword3 = new
                                                     {
                                                         value = time,
                                                     },
                                                     keyword4 = new
                                                     {
                                                         value = title,
                                                     },
                                                     remark = new
                                                     {
                                                         value = having
                                                     }
                                                 }
                                             }.ToJson());
            context.Response.Write(res);
        }

        public string SendMsg(HttpContext context, string postUrl, bool test)
        {
            // 获取 access token 
            var aModel = CacheHelper.Get<Token>("_AccessToken");

            // 参数传入以及处理
            //var openIds = (context.Request["openIds[]"] ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var msg = context.Request["msg"];
            var type = context.Request["msgType"].ToInt();
            var path = context.Request["path"];
            var title = context.Request["title"];
            var describe = context.Request["describe"];
            var twList = context.Request["twList"].Replace(@"\\", "/").JsonToObject<List<TwModel>>();
            
            if (!(aModel != null && !aModel.Value.IsNullOrEmpty() && aModel.Time.AddHours(2) >= DateTime.Now))
            {
                aModel = GetAModel();
            }
            //获取所有关注本公众号的用户
            var openIdJson = HttpHelper.HttpGet("https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + aModel.Value + "&next_openid=NEXT_OPENID").JsonToObject<OpenListModel>();
            var arr = new ArrayList(openIdJson.data.openid.Split(','));

            if (arr.Count < 2)
            {
                arr.Add(ConfigurationManager.AppSettings["OpenIdForAdmin"]);
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
                    touser = test ? arr[0] : arr,
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
                    touser = test ? arr[0] : arr,
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
                    touser = test ? arr[0] : arr,
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
                    // 插入重要信息
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
                        content_source_url = ConfigurationManager.AppSettings["Localhost"] + "/weixin/majorInfo.html?id=" + ident, // 原文的地址
                        author = "管理员",
                        tw.title,
                        content = tw.msg + "<br>===============================<br>请点击左下角(阅读原文),提交您的阅读结果", // 统一添加阅读原文提示
                        show_cover_pic = 1
                    });
                }
                var twBody = new
                {
                    articles
                };
                var twRes = HttpHelper.HttpPost("https://api.weixin.qq.com/cgi-bin/media/uploadnews?access_token=" + aModel.Value,
                                                twBody.ToJson());
                body = new
                {
                    touser = test ? arr[0] : arr,
                    mpnews = new
                    {
                        twRes.JsonToObject<MediaModel>().media_id
                    },
                    msgtype = "mpnews"
                };
            }
            var res = HttpHelper.HttpPost(postUrl + aModel.Value,
                                          body.ToJson());
            LogHelper.Log(body.ToJson(), postUrl + aModel.Value);
            return res;
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
            var openId = ConfigurationManager.AppSettings["OpenIdForAdmin"];
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
            var openId = ConfigurationManager.AppSettings["OpenIdForAdmin"] ?? "";
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

        class TemplateIdModel
        {
            public string errcode;
            public string errmsg;
            public string template_id;
        }
        class OpenListModel
        {
            public int total;
            public int count;
            public DataModel data;
            public string next_openid;

        }
        class DataModel {
            public string openid;
        }
    }
}