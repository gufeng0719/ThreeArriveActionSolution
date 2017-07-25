using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.WeiXinComm;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using Senparc.Weixin.MP.CommonAPIs;
using ThreeArriveAction.Web.UI;
using System.Web.SessionState;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// wxMenuManager 的摘要说明
    /// </summary>
    public class wxMenuManager : ManagePage,IHttpHandler, IRequiresSessionState
    {
        WeiXinCRMComm cpp = new WeiXinCRMComm();
        MenuMgr mMgr = new MenuMgr();
        public override void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "get":
                    GetMenu(context);
                    break;
                case "create":
                    Update(context);
                    break;
                case "del":
                    DelMenu(context);
                    break;
                case "flush":
                    FlushAT(context);
                    break;
            }
        }

        #region    获取最新的菜单============
        private void GetMenu(HttpContext context)
        {
            JsonMessage json = new JsonMessage();
            wx_userweixin weixin = GetWeiXinCode();
            string error = "";
            StringBuilder strJson = new StringBuilder();
            strJson.Append("{");
            string accessToken = cpp.getAccessToken(weixin.id, out error);
            if (error != "")
            {
                json.success = false;
                json.msg = error;
                context.Response.Write(json.ToJson());
                return;
            }
            GetMenuResult result = mMgr.GetMenu(accessToken);
            if (result == null)
            {
               
                json.success = false;
                json.msg = "未获得到菜单，请参考【使用规则】，自行排查问题！";
                context.Response.Write(json.ToJson());
                return;
                //强制刷新
                //accessToken = cpp.FlushAccessToken(weixin.id, out  error);
                //result = CommonApi.GetMenu(accessToken);
            }
            var topButtonList = result.menu.button;
            int topNum = topButtonList.Count;
            strJson.Append("\"success\":true,\"topNum\":"+topNum);
            strJson.Append(",\"rows\":[");
            for (int i = 0; i < topNum; i++)
            {
                var topButton = topButtonList[i];
                if (topButton != null)
                {
                    strJson.Append("{\"txtTop"+(i+1)+"\":"+topButton.name);
                    if (topButton.GetType() != typeof(SubButton))
                    {
                        //下面无子菜单
                        if (topButton.GetType() == typeof(SingleViewButton))
                        {
                            //View 页面跳转
                            strJson.Append(",\"txtTop" + (i + 1) + "Url\":" + ((SingleViewButton)topButton).url);
                            strJson.Append(",\"txtTop" + (i + 1) + "Key\":\"\"}");
                        }
                        else
                        {
                            strJson.Append(",\"txtTop" + (i + 1) + "Url\":\"\"");
                            strJson.Append(",\"txtTop" + (i + 1) + "Key\":" + ((SingleClickButton)topButton).key+"}");
                        }
                    }
                    else
                    {
                        //下面有子菜单
                        IList<SingleButton> subButtonList = ((SubButton)topButton).sub_button;
                        if (subButtonList != null && subButtonList.Count > 0)
                        {
                            strJson.Append(",\"subBtnList\":[");
                            for (int j = 0; j < subButtonList.Count; j++)
                            {
                                if (subButtonList[j].GetType() == typeof(SingleViewButton))
                                {
                                    SingleViewButton sub = (SingleViewButton)subButtonList[j];
                                    strJson.Append("{\"txtMenu" + (i + 1) + (j + 1) + "\":" + sub.name);
                                    strJson.Append(",\"txtMenu" + (i + 1) + (j + 1) + "Key\":\"\"");
                                    strJson.Append(",\"txtMenu" + (i + 1) + (j + 1) + "Url\":" + sub.url);
                                    strJson.Append("}");
                                }
                                else
                                {
                                    SingleClickButton sub = (SingleClickButton)subButtonList[j];
                                    strJson.Append("{\"txtMenu" + (i + 1) + (j + 1) + "\":" + sub.name);
                                    strJson.Append(",\"txtMenu" + (i + 1) + (j + 1) + "Key\":" + sub.key);
                                    strJson.Append(",\"txtMenu" + (i + 1) + (j + 1) + "Url\":\"\"");
                                    strJson.Append("}");
                                }
                                if (j != subButtonList.Count)
                                {
                                    strJson.Append(",");
                                }

                            }
                            strJson.Append("]");
                        }
                        strJson.Append("}");
                        if (i != topNum)
                        {
                            strJson.Append(",");
                        }
                    }
                    strJson.Append("}");
                }
                context.Response.Write(strJson.ToJson());
            }
        }
        #endregion

        #region 更新菜单=============
        public void Update(HttpContext context)
        {
            JsonMessage json = new JsonMessage();
            try
            {
                wx_userweixin weixin = GetWeiXinCode();
                string error = "";
                string accessToken = cpp.getAccessToken(weixin.id,out error);
                if (error != "")
                {
                    json.success = false;
                    json.msg = error;
                    context.Response.Output.Write(json.ToJson());
                    return;
                }
                //重新整理按钮信息
                ButtonGroup bg = new ButtonGroup();
                IList<BaseButton> topList = new List<BaseButton>();
                IList<SingleButton> subList = new List<SingleButton>();
                #region 菜单设置
                for (int i = 0; i < 3; i++)
                {
                    string txtNameValue = MXRequest.GetFormString("txtTop" + (i + 1));
                    string txtKey = MXRequest.GetFormString("txtTop" + (i + 1) + "Key");
                    string txtUrl = MXRequest.GetFormString("txtTop" + (i + 1) + "Url");
                    if (txtNameValue.Trim() == "")
                    {
                        //如果名称为空，则忽略该菜单，以及下面的子菜单
                        continue;
                    }
                    subList = new List<SingleButton>();
                    for(int j = 0; j < 5; j++)
                    {
                        #region 子菜单设置
                        string txtSubName = MXRequest.GetFormString("txtMenu"+(i+1)+(j+1));
                        string txtSubKey = MXRequest.GetFormString("txtMenu" + (i + 1) + (j + 1) + "Key");
                        string txtSubUrl = MXRequest.GetFormString("txtMenu" + (i + 1) + (j + 1) + "Url");
                        if (txtSubName.Trim() == "")
                        {
                            continue;
                        }
                        if (txtSubUrl.Trim() != "")
                        {
                            SingleViewButton sub = new SingleViewButton();
                            sub.name = txtSubName;
                            sub.url = txtSubUrl;
                            subList.Add(sub);
                        }else if (txtSubKey.Trim() != "")
                        {
                            SingleClickButton sub = new SingleClickButton();
                            sub.name = txtSubName;
                            sub.key = txtSubKey;
                            subList.Add(sub);
                        }
                        else
                        {
                            json.success = false;
                            json.msg = " 二级菜单的名称和Key或url必填!";
                            context.Response.Output.Write(json.ToJson());
                            return;
                        }
                        #endregion
                    }//子菜单循环结束

                    if (subList != null && subList.Count > 0)
                    {
                        //有子菜单，该一级菜单是SubButton
                        if (subList.Count < 1)
                        {
                            json.success = false;
                            json.msg = "子菜单个数为2~5个";
                            context.Response.Output.Write(json.ToJson());
                            return;
                        }
                        SubButton topButton = new SubButton(Utils.CutString(txtNameValue, 16));
                        topButton.sub_button.AddRange(subList);
                        topList.Add(topButton);
                    }
                    else
                    {
                        //无子菜单
                        if (txtKey.Trim() == "" && txtUrl.Trim() == "")
                        {
                            json.success = false;
                            json.msg = "若无子菜单，必须填写Key或者Url值！";
                            context.Response.Output.Write(json.ToJson());
                            return;
                        }
                        if (txtUrl.Trim() != "")
                        {
                            //view页面调整
                            SingleViewButton topSingleButton = new SingleViewButton();
                            topSingleButton.name = txtNameValue.Trim();
                            topSingleButton.url = txtUrl.Trim();
                            topList.Add(topSingleButton);
                        }else if (txtKey.Trim() != "")
                        {
                            SingleClickButton topSingleButton = new SingleClickButton();
                            topSingleButton.name = txtNameValue.Trim();
                            topSingleButton.key = txtKey.Trim();
                            topList.Add(topSingleButton);
                        }
                    }

                }
                #endregion
                bg.button.AddRange(topList);
                var result = mMgr.CreateMenu(accessToken, bg);
                json.success = true;
                json.msg = "菜单提交成功";
                context.Response.Output.Write(json.ToJson());
            }
            catch(Exception ex)
            {
                json.success = false;
                json.msg = "报错:"+ex.Message;
                context.Response.Output.Write(json.ToJson());
                
            }
        }
        #endregion

        #region 删除当前菜单
        /// <summary>
        /// 删除当前菜单
        /// </summary>
        /// <param name="context"></param>
        private void DelMenu(HttpContext context)
        {
            JsonMessage json = new JsonMessage();
            try
            {
                wx_userweixin weixin = GetWeiXinCode();
                string error = "";
                string token = cpp.getAccessToken(weixin.id, out error);
                if (error != "")
                {
                    json.success = false;
                    json.msg = error;
                    context.Response.Output.Write(json.ToJson());
                    return;

                }
                var result = CommonApi.DeleteMenu(token);
                //重新获得最新的菜单
                GetMenu(context);
                   
            }
            catch(Exception ex)
            {
                json.success = false;
                json.msg = "执行过程中出现错误:" + ex.Message;
                context.Response.Output.Write(json.ToJson());
            }

        }
        #endregion

        #region 刷新获取AccessToken
        private void FlushAT(HttpContext context)
        {
            JsonMessage json = new JsonMessage();
            try
            {
                wx_userweixin weixin = GetWeiXinCode();
                string error = "";
                cpp.FlushAccessToken(weixin.id, out error);
                if (error != "")
                {
                    json.success = false;
                    json.msg = error;
                    
                }
                else
                {
                    json.success = true;
                    json.msg = "Access_Token更新成功";
                }
                context.Response.Output.Write(json.ToJson());
            }catch(Exception ex)
            {
                json.success = false;
                json.msg = "执行过程中出现错误:" + ex.Message;
                context.Response.Output.Write(json.ToJson());
            }
        }
        #endregion
        public new bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
