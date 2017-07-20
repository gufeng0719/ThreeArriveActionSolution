using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ThreeArriveAction.Common;
using ThreeArriveAction.BLL;
using ThreeArriveAction.Model;
using ThreeArriveAction.Model;

namespace ThreeArriveAction.Web.UI
{
    public class ManagePage : System.Web.UI.Page
    {
        protected internal siteconfig siteConfig;
        
        public ManagePage()
        {
            
            siteConfig = new sys_configBLL().loadConfig();
            //ManagePage_Load();
        }

        //private void ManagePage_Load()
        //{
        //    //判断当前用户是否登录
        //    if (!IsUsersLogin())
        //    {
        //        Response.Write("<script>parent.location.href='" + siteConfig.webpath + siteConfig.webmanagepath + "/login.html'</script>");
        //        Response.End();
        //    }
        //}

        #region 登录用户============================================
        /// <summary>
        /// 判断当前用户是否已经登录(解决Session超时问题)
        /// </summary>
        public bool IsUsersLogin()
        {
            //如果Session为Null
            if (Session[MXKeys.SESSION_ADMIN_INFO] != null)
            {
                return true;
            }
            else
            {
                //检查Cookies
                string uphone = Utils.GetCookie("uphone", "ThreeArriveAction");
                string upwd = Utils.GetCookie("upwd", "ThreeArriveAction");
                if (uphone != "" && upwd != "")
                {
                    sys_UsersBLL bll = new sys_UsersBLL();
                    sys_UsersModel model = bll.GetUsersModelByUserPhone(uphone);
                    if (model != null)
                    {
                        if (string.Equals(model.UserPassword, upwd))
                        {
                            Session[MXKeys.SESSION_ADMIN_INFO] = model;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 取得登录人信息
        /// </summary>
        public sys_UsersModel GetUsersinfo()
        {
            if (IsUsersLogin())
            {
                sys_UsersModel model = Session[MXKeys.SESSION_ADMIN_INFO] as sys_UsersModel;
                if (model != null)
                {
                    return model;
                }
            }
            return null;
        }



        /// <summary>
        /// 检查用户权限
        /// </summary>
        /// <param name="nav_name">菜单名称</param>
        /// <param name="action_type">操作类型</param>
        public void ChkAdminLevel(string nav_name, string action_type)
        {
            //Model.manager model = GetAdminInfo();
            //BLL.manager_role bll = new BLL.manager_role();
            //bool result = bll.Exists(model.role_id, nav_name, action_type);

            //if (!result)
            //{
            //    string msgbox = "parent.jsdialog(\"错误提示\", \"您没有管理该页面的权限，请勿非法进入！\", \"back\", \"Error\")";
            //    Response.Write("<script type=\"text/javascript\">" + msgbox + "</script>");
            //    Response.End();
            //}
        }

        /// <summary>
        /// 写入管理日志
        /// </summary>
        /// <param name="action_type"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool AddAdminLog(string action_type, string remark)
        {
            //if (siteConfig.logstatus > 0)
            //{
            //    Model.manager model = GetAdminInfo();
            //    int newId = new BLL.manager_log().Add(model.id, model.user_name, action_type, remark);
            //    if (newId > 0)
            //    {
            //        return true;
            //    }
            //}
            return false;
        }

        #endregion

        #region JS提示============================================
        /// <summary>
        /// 添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="msgcss">CSS样式</param>
        protected void JscriptMsg(string msgtitle, string url, string msgcss)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", \"" + msgcss + "\")";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }
        /// <summary>
        /// 带回传函数的添加编辑删除提示
        /// </summary>
        /// <param name="msgtitle">提示文字</param>
        /// <param name="url">返回地址</param>
        /// <param name="msgcss">CSS样式</param>
        /// <param name="callback">JS回调函数</param>
        protected void JscriptMsg(string msgtitle, string url, string msgcss, string callback)
        {
            string msbox = "parent.jsprint(\"" + msgtitle + "\", \"" + url + "\", \"" + msgcss + "\", " + callback + ")";
            ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
        }

        #endregion

        /*
        public bool IsWeiXinCode()
        {
             
            //如果Session为Null
            if (Session["nowweixin"] != null)
            {
                return true;
            }
            else
            {
                //检查Cookies
                string uweixinId = Utils.GetCookie("nowweixinId", "ThreeArriveAction");
                if (uweixinId != "")
                {
                    BLL.wx_userweixin bll = new BLL.wx_userweixin();
                    Model.wx_userweixin model = bll.GetModel(int.Parse(uweixinId));
                    if (model != null)
                    {
                        Session["nowweixin"] = model;
                        return true;
                    }
                }
            }
            return false;
        }*/

        /// <summary>
        /// 取得当前微信帐号信息
        /// </summary>
        public wx_userweixin GetWeiXinCode()
        {
            wx_userweixin weixin = new wx_userweixin();
            weixin.id = 1;
            weixin.wxName = "淮安区三到行动";
            weixin.wxId = "gh_9d7c970705cb";
            weixin.weixinCode = "haqsdxd";
            weixin.wxToken = "Token2017";
            weixin.AppId = "wx7c32c023241aba89";
            weixin.AppSecret = "ed9fa9aaf2c12f9e110a12b83f8d9d08";
            return weixin;
        }

        

    }
}
