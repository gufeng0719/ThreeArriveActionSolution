using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using System.Text;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Common;
using ThreeArriveAction.Web.UI;
using System.Data;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// wxrequestRulesManager 的摘要说明
    /// </summary>
    public class wxrequestRulesManager :  IHttpHandler, IRequiresSessionState
    {
        private readonly wx_requestRuleBLL rBLL = new wx_requestRuleBLL();
        private readonly wx_requestRuleContentBLL rcBLL = new wx_requestRuleContentBLL();
        public void ProcessRequest(HttpContext context)
        {
            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "search":
                    ShowRequestRule(context);
                    break;
                case "save":
                    SaveRequestRule(context);
                    break;
                case "delete":
                    DeleteRequestRule(context);
                    break;
                case "get":
                    GetRequestContent(context);
                    break;

            }

        }


        private void ShowRequestRule(HttpContext context)
        {
            //Model.manager manager = GetAdminInfo();
            ManagePage mPage = new ManagePage();
            wx_userweixin weixin = mPage.GetWeiXinCode();
            StringBuilder strJson = new StringBuilder();
            string appId = weixin.AppId;
            string act = MXRequest.GetQueryString("act");
            string reqestType = "";
            strJson.Append("{");
            if (act == "subscribe")//关注时
            {
                reqestType = "6";
                strJson.Append("\"reqestType\":6,\"reqestText\":\"关注时回复\"");
            }
            else if (act == "default")//默认回复
            {
                reqestType = "0";
                strJson.Append("\"reqestType\":0,\"reqestText\":\"默认回复\"");
            }
            else if(act=="cancel")//取消关注时
            {
                reqestType = "7";
                strJson.Append("\"reqestType\":7,\"reqestText\":\"取消关注时回复\"");
            }
            IList<wx_requestRuleModel> ruleList = rBLL.GetModelList("wId=" + weixin.id + " and reqestType=" + reqestType);
            if (ruleList != null && ruleList.Count > 0 && ruleList[0] != null)
            {
                //hidId.Value = ruleList[0].id.ToString();
                strJson.Append(",\"rId\":"+ruleList[0].id);
                wx_requestRuleContentModel rc = new wx_requestRuleContentModel();
                switch (ruleList[0].responseType)
                {
                    case 1:
                        //纯文本
                        rc = rcBLL.GetModelList("rId=" + ruleList[0].id)[0];
                        strJson.Append(",\"responseType\":0");
                        strJson.Append(",\"txtContent\":\"" + rc.rContent + "\"");
                        break;
                    case 2:
                        //图文
                        strJson.Append(",\"responseType\":1");
                        DataTable rclist = rcBLL.GetList("rId=" + ruleList[0].id + " order by seq").Tables[0];
                        if (rclist.Rows.Count > 0)
                        {
                            strJson.Append(",\"rows\":" + JsonHelper.ToJson(rclist));
                        }
                        else
                        {
                            strJson.Append(",\"rows\":[]");
                        }
                        break;
                    case 3:
                        //语音
                        rc = rcBLL.GetModelList("rId=" + ruleList[0].id)[0];
                        strJson.Append(",\"responseType\":2");
                        strJson.Append(",\"musicFile\":\""+rc.mediaUrl+"\"");
                        strJson.Append(",\"musicTitle\":\""+rc.rContent+"\"");
                        strJson.Append(",\"musicRemark\":\""+rc.remark+"\"");
                        
                        break;
                    default:
                        strJson.Append(",\"responseType\":0");
                        strJson.Append(",\"txtContent\":\"" + rc.rContent + "\"");
                        break;

                }

            }
            else
            {
                strJson.Append(",\"responseType\":0");
                strJson.Append(",\"txtContent\":\"\"");
            }
            strJson.Append("}");
            context.Response.Write(strJson.ToString());
        }

        private void SaveRequestRule(HttpContext context)
        {
            int requestType = MXRequest.GetFormIntValue("requestType");
            string json = "";
            string ruleName = "";
            try
            {
                if (requestType == 6)
                {
                    ruleName = "关注时的触发内容";
                }
                else if (requestType == 0)
                {
                    ruleName = "默认回复内容";
                }else if (requestType==7)
                {
                    ruleName = "取消关注的触发内容";
                }
                sys_UsersModel usersModel = new ManagePage().GetUsersinfo();
                wx_userweixin weixin = new ManagePage().GetWeiXinCode();
                rBLL.DeleteRule(MXRequest.GetFormIntValue("rId"));
                wx_requestRuleModel rule = new wx_requestRuleModel();
                rule.uId = usersModel.UserId;
                rule.wId = weixin.id;
                rule.ruleName = ruleName;
                rule.reqestType = requestType;
                rule.isDefault = false;
                rule.createDate = DateTime.Now;
                wx_requestRuleContentModel rc ;
                string responseType = MXRequest.GetFormString("responseType");
                if (responseType == "0")
                {//纯文本
                    rc = new wx_requestRuleContentModel();
                    rule.responseType = 1;//回复类型:文本1,图文:2,语言:3,视频:4,第三方接口5
                    int rId = rBLL.Add(rule);
                    rc.rId = rId;
                    rc.uId = usersModel.UserId;
                    string txtContent = MXRequest.GetFormString("txtContent");
                    rc.rContent = txtContent;
                    rc.createDate = DateTime.Now;
                    rcBLL.Add(rc);
                    new ManagePage().AddAdminLog(MXEnums.ActionEnum.Edit.ToString(), "编辑" + ruleName); //记录日志
                    json = "{\"info\":\"编辑" + ruleName + "成功！\",\"status\":\"y\"}";
                }
                else if (responseType == "1")
                {//图文
                    rule.responseType = 2;
                    int rId = rBLL.Add(rule);
                    rc = new wx_requestRuleContentModel();
                    rc.rId = rId;
                    rc.uId = usersModel.UserId;
                    rc.rContent = MXRequest.GetFormString("txtTitle");
                    rc.picUrl = MXRequest.GetFormString("txtImgUrl");
                    rc.rContent2 = MXRequest.GetFormString("txtContent");
                    rc.detailUrl = MXRequest.GetFormString("txtUrl");
                    rc.seq = MXRequest.GetFormIntValue("txtSortId");
                    rc.createDate = DateTime.Now;
                    rcBLL.Add(rc);
                    new ManagePage().AddAdminLog(MXEnums.ActionEnum.Edit.ToString(), "编辑" + ruleName); //记录日志
                    json = "{\"info\":\"编辑" + ruleName + "成功！\",\"status\":\"y\"}";
                }
                else if (responseType == "3")
                {
                    //语音
                    //规则
                    rule.responseType = 3;//回复的类型:文本1，图文2，语音3，视频4,第三方接口5
                    int rId = rBLL.Add(rule);
                    rc = new wx_requestRuleContentModel();
                    //内容
                    rc.rId = rId;
                    rc.uId = usersModel.UserId;
                    rc.createDate = DateTime.Now;
                    rc.mediaUrl =MXRequest.GetFormString("txtMusicFile");
                    rc.rContent = MXRequest.GetFormString("txtMusicTitle");
                    rc.remark = MXRequest.GetFormString("txtMusicRemark");
                    rcBLL.Add(rc);
                   new ManagePage().AddAdminLog(MXEnums.ActionEnum.Edit.ToString(), "编辑" + ruleName); //记录日志
                     json = "{\"info\":\"编辑" + ruleName + "成功！\",\"status\":\"y\"}";
                }
            }
            catch 
            {
                 json = "{\"info\":\"编辑" + ruleName + "失败！\",\"status\":\"n\"}";
            }
            context.Response.Write(json);
        }

        private void GetRequestContent(HttpContext context)
        {
            int id = MXRequest.GetQueryIntValue("id");
            wx_requestRuleContentModel rc = rcBLL.GetModel(id);
            context.Response.Write(rc.ToJson());
        }

        private void DeleteRequestRule(HttpContext context)
        {
            int id = MXRequest.GetFormInt("id");
            string json = "" ;
            if (rcBLL.Delete(id))
            {
                json = "{\"status\":\"y\"}";
            }
            else
            {
                json = "{\"status\":\"n\"}";
            }
            context.Response.Write(json);
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