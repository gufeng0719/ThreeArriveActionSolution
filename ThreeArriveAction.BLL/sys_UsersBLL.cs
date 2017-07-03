using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ThreeArriveAction.Common;
using ThreeArriveAction.Model;
using ThreeArriveAction.DAL;

namespace ThreeArriveAction.BLL
{
    public partial class sys_UsersBLL : System.Web.UI.Page
    {
        private readonly sys_UsersDAL userDAL = new sys_UsersDAL();
        #region 登录功能
        public string Login(string userPhone, string userPassword)
        {
            JsonMessage json = new JsonMessage();
            if (Session["AdminLoginSun"] == null)
            {
                Session["AdminLoginSun"] = 1;
            }
            else
            {
                Session["AdminLoginSun"] = Convert.ToInt32(Session["AdminLoginSun"]) + 1;
            }
            //判断登录错误次数
            if (Session["AdminLoginSun"] != null && Convert.ToInt32(Session["AdminLoginSun"]) > 5)
            {
                return "错误超过5次，关闭浏览器重新登录！"; ;
            }

            sys_UsersModel userModel = GetUsersModelByUserPhone(userPhone);
            if (userModel != null)
            {
                if (string.Equals(userModel.UserPassword, userPassword))
                {
                    Session[MXKeys.SESSION_ADMIN_INFO] = userModel;
                    Session.Timeout = 45;
                    //写入登录日志
                    siteconfig siteConfig = new sys_configBLL().loadConfig();
                    if (siteConfig.logstatus > 0)
                    {
                        // new BLL.manager_log().Add(model.id, model.user_name, MXEnums.ActionEnum.Login.ToString(), "用户登录");
                    }
                    //写入Cookies
                    Utils.WriteCookie("DTRememberName",Utils.UrlEncode(userModel.UserName), 14400);
                    Utils.WriteCookie("uphone", "ThreeArriveAction", userModel.UserPhone);
                    Utils.WriteCookie("upwd", "ThreeArriveAction", userModel.UserPassword);
                    json.success = true;
                    json.msg = "登录成功";
                }
                else
                {
                    json.success = false;
                    json.msg = "密码不正确";
                }
            }
            else
            {
                json.success = false;
                json.msg = "该用户不存在";
            }
            return JsonHelper.ToJson(json);
        }
        #endregion
        #region 添加
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userModel">用户实体</param>
        /// <returns></returns>
        public string AddUser(sys_UsersModel userModel)
        {
            int rows = userDAL.AddUser(userModel);
            if (rows > 0)
            {
                return "{\"info\":\"用户添加成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"用户添加失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 修改
        public string UpdateUser(sys_UsersModel userModel)
        {
            bool bl = userDAL.Update(userModel);
            if (bl)
            {
                return "{\"info\":\"用户修改成功\",\"status\":\"y\"}";
            }
            else
            {
                return "{\"info\":\"用户修改失败\",\"status\":\"n\"}";
            }
        }
        #endregion

        #region 删除
        #endregion

        #region 查询
        /// <summary>
        /// 根据用户电话查询改用户实体信息
        /// </summary>
        /// <param name="userPhone"></param>
        /// <returns></returns>
        public sys_UsersModel GetUsersModelByUserPhone(string userPhone)
        {
            sys_UsersModel userModel = userDAL.GetModelByUserPhone(userPhone);
            return userModel;
        }
        /// <summary>
        /// 查询前几条用户信息
        /// </summary>
        /// <param name="top">条数</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序</param>
        /// <returns></returns>
        public DataSet  GetList(int top,string strWhere,string filedOrder)
        {
            DataSet ds = userDAL.GetList(top,strWhere,filedOrder);
            return ds;
        }

        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="PageIndex">当前页码</param>
        /// <param name="strWhere">查询条件</param>
        /// <param name="filedOrder">排序条件</param>
        /// <param name="recordCount">总记录数</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize,int PageIndex,string strWhere,string filedOrder,out int recordCount)
        {
            DataSet ds = userDAL.GetList(pageSize, PageIndex, strWhere, filedOrder, out recordCount);
            return ds;
        }

        #endregion
    }
}
