using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Common;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using System.Data;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_UsersManager 的摘要说明
    /// </summary>
    public class sys_UsersManager : IHttpHandler, IRequiresSessionState
    {
        private readonly sys_UsersBLL usersBLL = new sys_UsersBLL();
        public void ProcessRequest(HttpContext context)
        {


            string type = MXRequest.GetQueryString("type");
            switch (type)
            {
                case "login":
                    Login(context);
                    break;
                case "exit":
                    Exit(context);
                    break;
                case "get":
                    GetUsersList(context);
                    break;
            }
        }
        #region 登录
        private void Login(HttpContext context)
        {
            string uphone = context.Request.Form["uphone"].ToString();
            string upwd = context.Request.Form["upwd"].ToString();
            string result = usersBLL.Login(uphone,upwd);
            context.Response.Write(result);
        }
        #endregion
        #region 退出
        private void Exit(HttpContext context)
        {
            try
            {
               
               context.Session[MXKeys.SESSION_ADMIN_INFO] = null;
               Utils.WriteCookie("uphone", "ThreeArriveAction", -14400);
               Utils.WriteCookie("upwd", "ThreeArriveAction", -14400);

               //context.Session["yubomId"] = null;
               // Utils.WriteCookie("yubomId", "MxWeiXinPF", -14400);
                context.Response.Write("{\"success\":true}");
            }
            catch (Exception)
            {
                context.Response.Write("{\"success\":false}");
            }


            
        }
             
        #endregion
        #region 分页查询
        public void GetUsersList(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                StringBuilder strSql = new StringBuilder();
                int pageSize = 10;
                int page = MXRequest.GetQueryInt("page", 1);
                string key = MXRequest.GetQueryString("keywords");
                key = key.Replace("'", "");
                //区管理员 能够查询所有人员
                if (model.OrganizationId == 1)
                {
                    strSql.Append(" 1=1 ");
                }
                else if (model.OrganizationId == 2)
                {
                    //镇组织委员 只能查询镇下行政村的人员
                    strSql.Append("VillageId in (select ViilageId from sys_Villages where VillageId="+model.VillageId+" or VillageParId =)"+model.VillageId);
                }
                else
                {
                    //村负责人 只能查询本村的人员
                    strSql.Append(" VillageId =" + model.VillageId);
                }
                if (!string.IsNullOrEmpty(key))
                {
                    strSql.Append(" and ( UserPhone like '%"+key+"%' or UserName like '%"+key+"%' or UserDuties like '%"+key+"%' ) ");
                }
                int totalCount=0;
                StringBuilder strJson = new StringBuilder();
                
                DataSet ds = usersBLL.GetList(pageSize,page,strSql.ToString(),"UserId asc",out totalCount);
                strJson.Append("{\"total\":" + totalCount);
                if (totalCount > 0)
                {
                   strJson.Append(",\"rows\":"+ JsonHelper.ToJson(ds.Tables[0]));
                }
                string pageContent = Utils.OutPageList(pageSize, page, totalCount, "Load(__id__)", 8);
                if (pageContent == "")
                {
                    strJson.Append(",\"pageContent\":\"\"");
                }
                else
                {
                    strJson.Append(",\"pageContent\":" + pageContent);
                }
                
                strJson.Append("}");
                context.Response.Write(strJson.ToString());
            }
        }
        #endregion
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}