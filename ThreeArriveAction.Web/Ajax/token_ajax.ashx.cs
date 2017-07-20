using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.WeiXinComm.CustomMessageHandler;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// token_ajax 的摘要说明
    /// </summary>
    public class token_ajax : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            ManagePage page = new ManagePage();
            wx_userweixin weixin = page.GetWeiXinCode();
            string Token = weixin.wxToken;
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            string echostr = context.Request["echostr"];

            if (context.Request.HttpMethod == "GET")
            {
                //get method - 仅在微信后台填写URL验证时触发
                if (CheckSignature.Check(signature, timestamp, nonce, Token))
                {
                    context.Response.Output.Write(echostr); //返回随机字符串则表示验证通过
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
                    context.Response.Output.Write("参数错误！");
                    return;
                }

                //本地测试的时候注释掉 ----end -----
                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
                int maxRecordCount = 10;
                PostModel postModel = new PostModel();
                postModel.AppId = weixin.AppId;
                postModel.Token = weixin.wxToken;
                postModel.EncodingAESKey = "jtU2xBgEnE8bQayvnpLR0KAp9JcmypG2Eq4J3qmSfQp";
                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
                var messageHandler = new CustomMessageHandler(context.Request.InputStream,postModel, maxRecordCount);

                try
                {
                    //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
                    //messageHandler.RequestDocument.Save(
                    //    Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" +
                    //                   messageHandler.RequestMessage.FromUserName + ".txt"));
                    //执行微信处理过程
                    messageHandler.Execute();
                    //测试时可开启，帮助跟踪数据
                    //messageHandler.ResponseDocument.Save(
                    //    Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" +
                    //                   messageHandler.ResponseMessage.ToUserName + ".txt"));
                    context.Response.Output.Write(messageHandler.ResponseDocument.ToString());
                    return;
                }
                catch (Exception ex)
                {
                    Page webpage = new Page();
                    using (TextWriter tw = new StreamWriter(webpage.Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
                    {
                        tw.WriteLine(ex.Message);
                        tw.WriteLine(ex.InnerException.Message);
                        if (messageHandler.ResponseDocument != null)
                        {
                            tw.WriteLine(messageHandler.ResponseDocument.ToString());
                        }
                        tw.Flush();
                        tw.Close();
                    }
                    context.Response.Output.Write("");
                }
                finally
                {
                    context.Response.End();
                }
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