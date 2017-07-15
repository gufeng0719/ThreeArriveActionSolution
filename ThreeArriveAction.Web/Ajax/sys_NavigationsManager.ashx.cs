using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
using ThreeArriveAction.Common;
using System.Data;
using ThreeArriveAction.BLL;

namespace ThreeArriveAction.Web.Ajax
{
    /// <summary>
    /// sys_NavigationsManager 的摘要说明
    /// </summary>
    public class sys_NavigationsManager : IHttpHandler,IRequiresSessionState
    {
        private readonly sys_NavigationsBLL navBLL = new sys_NavigationsBLL();
        public void ProcessRequest(HttpContext context)
        {
            
            string type = context.Request.QueryString["type"].ToString();
            switch (type)21
            {
                case "get":
                    GetNavigationByOrgan(context);
                    break;
                case "query":
                    GetNavigationListJson(context);
                    break;
                case "select":
                    GetListTreeJson(context);
                    break;
                case "edit":
                    GetNavigationJson(context);
                    break;
                case "save":
                    SaveNavigation(context);
                    break;
            }   
        }

        #region 获取该菜单信息
        private void GetNavigationJson(HttpContext context)
        {
            int navid = int.Parse(MXRequest.GetQueryString("navid"));
            string result = navBLL.GetNavigationsJsonByNavId(navid);
            context.Response.Write(result);
        }
        #endregion

        #region  获取后台导航字符串
        private void GetNavigationByOrgan(HttpContext context)
        {
            //获取当前登录的操作者信息
            sys_UsersModel model = new ManagePage().GetUsersinfo();
            if (model == null)
            {
                context.Response.Write("<script>parent.location.href='login.html'</script>");
            }
            else
            {
                List<sys_OrganizationANDNavigationsModel> modelList = new sys_OrganizationAndNavigationsBLL().GetOrganizationNavigation(model.OrganizationId);
                DataTable dt = navBLL.GetList(0);
                GetNavigationChilds(context, dt, 0, modelList);
            }
        }

        private void GetNavigationChilds(HttpContext context,DataTable oldData,int parentId,List<sys_OrganizationANDNavigationsModel> ls)
        {
            DataRow[] dr = oldData.Select("ParentId="+parentId);
            bool isWrite=false;
            for (int i = 0; i < dr.Length; i++)
            {
                //检查是否显示在界面上
                bool isActionPass = true;
                sys_OrganizationANDNavigationsModel model = ls.Find(p=>p.NavigationId ==int.Parse(dr[i]["NavigationId"].ToString()));
                if (model == null)
                {
                    isActionPass = false;
                }
                //如果没有权限则不显示
                if (!isActionPass)
                {
                    if (isWrite && i == (dr.Length - 1) && parentId > 0)
                    {
                        context.Response.Write("</ul>\n");
                    }
                    continue;
                }
                //输出开始标记
                if (i == 0 && parentId > 0)
                {
                    isWrite = true;
                    context.Response.Write("<ul>\n");
                }
                //以下是输出选项内容====
                //根目录
                if (int.Parse(dr[i]["parentId"].ToString()) == 0)
                {
                    context.Response.Write("<div class=\"list-group\" name=\"" + dr[i]["NavigationName"].ToString() + "\">\n");
                    if (dr[i]["NavigationName"].ToString() != "sys_contents")
                    {
                        context.Response.Write("<h2>" + dr[i]["NavigationName"].ToString() + "<i></i></h2>\n");
                    }
                    //调用自身迭代
                    this.GetNavigationChilds(context, oldData, int.Parse(dr[i]["NavigationId"].ToString()), ls);
                    context.Response.Write("</div>\n");
                }
                else
                {
                    context.Response.Write("<li>\n");
                    context.Response.Write("<a navid=\""+dr[i]["NavigationId"].ToString()+"\"");
                    if (!string.IsNullOrEmpty(dr[i]["NavUrl"].ToString()))
                    {
                        context.Response.Write(" href=\""+dr[i]["NavUrl"].ToString()+"\" target=\"mainframe\"");
                    }
                    context.Response.Write(" class=\"item\">\n");
                    context.Response.Write("<span>" + dr[i]["NavigationName"].ToString() + "</span>\n");
                    context.Response.Write("</a>\n");
                    //调用自身迭代
                    this.GetNavigationChilds(context, oldData, int.Parse(dr[i]["NavigationId"].ToString()), ls);
                    context.Response.Write("</li>\n");
                }
                //以上是输出选项内容
                //输出结束标记
                if (i == (dr.Length - 1) && parentId > 0)
                {
                    context.Response.Write("</ul>\n");
                }
            }
        }
        #endregion

        #region  菜单列表数据
        /// <summary>
        /// 获取所有菜单列表
        /// </summary>
        /// <param name="context"></param>
        private void GetNavigationListJson(HttpContext context)
        {
            string result = navBLL.GetDataListJson(0);
            context.Response.Write(result);
        }
        #endregion

        #region 菜单下列列表数据
        /// <summary>
        /// 获取用于下来选择的单选数据列表
        /// </summary>
        /// <param name="context"></param>
        private void GetListTreeJson(HttpContext context)
        {
            string result = navBLL.GetListTreeJson(0);
            context.Response.Write(result);
        }
        #endregion

        #region 保存菜单信息
        private void SaveNavigation(HttpContext context)
        {
            sys_NavigationsModel model = new sys_NavigationsModel();
            model.NavigationName = MXRequest.GetFormString("navname");
            model.NavUrl = MXRequest.GetFormString("navurl");
            model.Reamrks = MXRequest.GetFormString("remark");
            model.NavState = 1;
            model.NavIcon = null;
            model.ParentId = int.Parse(MXRequest.GetFormString("parentid"));
            string action = MXRequest.GetFormString("action");
            
            string result = "";
            if (action == "add")
            {
                result = navBLL.AddNavigation(model);
            }
            else
            {
                model.NavigationId = int.Parse(MXRequest.GetFormString("navid"));
                result = navBLL.UpdateNavigation(model);

            }
            context.Response.Write(result);
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