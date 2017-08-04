using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.BLL;
using ThreeArriveAction.WeiXinComm.CustomMessageHandler;
using Senparc.Weixin;
using ThreeArriveAction.Common;
using System.Xml;
using System.Text;
using ThreeArriveAction.WeiXinComm.Utilities;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// token_ajax 的摘要说明
    /// </summary>
    public class token_ajax : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            Page webpage = new Page();
            ManagePage page = new ManagePage();
            wx_userweixin weixin = page.GetWeiXinCode();
            string Token = weixin.wxToken;
            LogHelper.Log(context.Request.HttpMethod, "请求类型");
            LogHelper.Log( context.Request["signature"]+"||"+ context.Request["timestamp"]+"||"+ context.Request["nonce"],"请求数据");
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            //string echostr = context.Request["echostr"];
            
            if (context.Request.HttpMethod == "GET")
            {
                //get method - 仅在微信后台填写URL验证时触发
                if (CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    context.Response.Output.Write(""); //返回随机字符串则表示验证通过
                }
                else
                {
                    context.Response.Output.Write("failed:" + signature + ",token:" + Token + " " + CheckSignature.GetSignature(timestamp, nonce, Token) + "。" +
                                "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
                context.Response.End();
            }
            else
            {
                //post method - 当有用户想公众账号发送消息时触发
                //本地测试的时候注释掉 ----start -----

                if (!CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    LogHelper.Log("出现错误","参数错误");
                    context.Response.Output.Write("参数错误！");
                }

                //本地测试的时候注释掉 ----end -----
                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                int maxRecordCount = 10;
                PostModel postModel = new PostModel();
                postModel.AppId = weixin.AppId;
                postModel.Token = weixin.wxToken;
                postModel.Signature = signature;
                postModel.Timestamp = timestamp;
                postModel.Nonce = nonce;
                
                postModel.EncodingAESKey = "jtU2xBgEnE8bQayvnpLR0KAp9JcmypG2Eq4J3qmSfQp";
                try
                {
                    WXOpera(context,postModel);
                }catch(Exception ex)
                {
                    LogHelper.Log(ex.Message, "错误信息");
                }
            }
        }

        /// <summary>
        /// 微信操作
        /// </summary>
        private void WXOpera(HttpContext context,PostModel postModel)
        {
            WxMessage wx = GetWxMessage(context);
            string res = "";
            if (!string.IsNullOrEmpty(wx.EventName) && wx.EventName.Trim() == "subscribe")
            {
                //刚关注时的时间，用于欢迎词
                wx_requestRuleBLL rBLL = new wx_requestRuleBLL();
                wx_requestRuleContentBLL rcBLL = new wx_requestRuleContentBLL();
                wx_requestRuleModel rModel = rBLL.GetModelList("reqestType=6")[0];
                wx_requestRuleContentModel rcModel = rcBLL.GetModelList(" rId="+rModel.id)[0];
                if (rModel.responseType == 1)
                {
                    //文本回复
                    string content = "";
                    content = rcModel.rContent;
                    res = sendTextMessage(wx, content);
                }
                else if (rModel.responseType == 2)
                {
                    var accessToken = Senparc.Weixin.MP.Containers.AccessTokenContainer.TryGetAccessToken(postModel.AppId, "ed9fa9aaf2c12f9e110a12b83f8d9d08");
                    var uploadResult = Senparc.Weixin.MP.AdvancedAPIs.MediaApi.UploadTemporaryMedia(accessToken, UploadMediaFileType.image,
                                                                 Server.GetMapPath("~" + rcModel.picUrl));
                    
                    //图片回复
                    string meadid = uploadResult.media_id;
                    res = sendImageMessage(wx, meadid);
                }
                
                LogHelper.Log(res,"输出");
                HttpContext.Current.Response.Write(res);
                HttpContext.Current.Response.End();
            }
        }

        /// <summary>
        /// 获取和设置微信类中的信息
        /// </summary>
        /// <returns></returns>
        private WxMessage GetWxMessage(HttpContext context)
        {
            WxMessage wx = new WxMessage();
            StreamReader str = new StreamReader(context.Request.InputStream, Encoding.UTF8);
            XmlDocument xml = new XmlDocument();
            xml.Load(str);
            str.Close();
            str.Dispose();
            wx.ToUserName = xml.SelectSingleNode("xml").SelectSingleNode("ToUserName").InnerText;
            wx.FromUserName = xml.SelectSingleNode("xml").SelectSingleNode("FromUserName").InnerText;
            wx.MsgType = xml.SelectSingleNode("xml").SelectSingleNode("MsgType").InnerText;
            if (wx.MsgType.Trim() == "text")
            {
                wx.Content = xml.SelectSingleNode("xml").SelectSingleNode("Content").InnerText;
            }
            if (wx.MsgType.Trim() == "event")
            {
                wx.EventName = xml.SelectSingleNode("xml").SelectSingleNode("Event").InnerText;
                wx.EventKey = xml.SelectSingleNode("xml").SelectSingleNode("EventKey").InnerText;
            }
            return wx;
        }

        /// <summary>  
        /// 发送文字消息  
        /// </summary>  
        /// <param name="wx" />获取的收发者信息  
        /// <param name="content" />内容  
        /// <returns></returns>  
        private string sendTextMessage(WxMessage wx, string content)
        {
            string res = string.Format(Message_Text,
                wx.FromUserName, wx.ToUserName, DateTime.Now.Ticks, content);
            return res;
        }

        private string sendImageMessage(WxMessage wx,string meadid)
        {
            string res = string.Format(Message_Image, wx.FromUserName, wx.ToUserName, DateTime.Now.Ticks, meadid);
            return res;
        }
        /// <summary>
        /// 普通文本消息
        /// </summary>
        private static string Message_Text
        {
            get
            {
                return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>";
            }
        }

        private static string Message_Image
        {
            get
            {
                return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[image]]></MsgType>
                            <Image>
                            <MediaId><![CDATA[{3}]]></MediaId>
                            </Image>
                            </xml>";
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