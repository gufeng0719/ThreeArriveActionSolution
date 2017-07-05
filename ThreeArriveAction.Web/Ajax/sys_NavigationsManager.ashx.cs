using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using ThreeArriveAction.Model;
using ThreeArriveAction.Web.UI;
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
            switch (type)
            {
                case "get":
                    GetNavigationByOrgan(context);
                    break;
                case "query":
                    GetNavigationListJson(context);
                    break;

            }
            
        }

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

        private void GetNavigationListJson(HttpContext context)
        {
            string result = navBLL.GetDataListJson(0);
            context.Response.Write(result);
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